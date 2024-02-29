using LibGit2Sharp;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Build.Scripts.Azure
{
    public class AzureAccess : IDisposable
    {
        private readonly string _uri;
        private readonly string _token;
        private VssConnection? _connection;

        public AzureAccess(string uri, string token)
        {
            _uri = uri;
            _token = token;
        }

        public async Task Connect()
        {
            var creds = new VssBasicCredential(string.Empty, _token);
            _connection = new VssConnection(new Uri(_uri), creds);
            await _connection.ConnectAsync();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private VssConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("AzureAccess - Call Connect before any operation!");
                }
                return _connection;
            }
        }

        public async Task GetRepository(string project, string repository, string branchName, string workdirPath)
        {
            var gitClient = Connection.GetClient<GitHttpClient>();
            var repo = await gitClient.GetRepositoryAsync(project, repository);

            var sourceUrl = repo.RemoteUrl;

            if (Directory.Exists(workdirPath))
            {
                using (var gitRepo = new Repository(workdirPath))
                {
                    Fetch(gitRepo);
                    Reset(gitRepo);
                    if (gitRepo.Head.FriendlyName == branchName)
                    {
                        Pull(gitRepo);
                    }
                    else
                    {
                        Checkout(gitRepo, branchName);
                    }
                }
            }
            else
            {
                Clone(sourceUrl, workdirPath, branchName);
            }
        }

        private void Clone(string sourceUrl, string workdirPath, string branchName)
        {
            var options = new CloneOptions
            {
                Checkout = true,
                BranchName = branchName
            };

            options.FetchOptions.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
            {
                Username = "pat",
                Password = _token
            };

            Repository.Clone(sourceUrl, workdirPath, options);
        }

        private void Fetch(Repository gitRepo)
        {
            var options = new FetchOptions
            {
                TagFetchMode = TagFetchMode.Auto
            };

            options.CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
            {
                Username = "pat",
                Password = _token
            };

            var remote = gitRepo.Network.Remotes["origin"];
            var msg = "Fetching remote";
            var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
            Commands.Fetch(gitRepo, remote.Name, refSpecs, options, msg);
        }

        private void Reset(Repository gitRepo)
        {
            var currentCommit = gitRepo.Head.Tip;
            gitRepo.Reset(ResetMode.Hard, currentCommit);
        }

        private void Pull(Repository gitRepo)
        {
            var options = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                    {
                        Username = "pat",
                        Password = _token
                    }
                },
                MergeOptions = new MergeOptions
                {
                    FastForwardStrategy = FastForwardStrategy.FastForwardOnly,
                    FileConflictStrategy = CheckoutFileConflictStrategy.Theirs,
                    MergeFileFavor = MergeFileFavor.Theirs,
                }
            };

            var signature = new Signature(new Identity("Scripts Service", "script@script.com"), DateTimeOffset.Now);
            Commands.Pull(gitRepo, signature, options);
        }

        private void Checkout(Repository gitRepo, string branchName)
        {
            var branch = gitRepo.Branches
                .FirstOrDefault(b => b.FriendlyName == $"{b.RemoteName}/{branchName}");

            if (branch == null)
            {
                throw new Exception($"Branch name \"{branchName}\" not found!");
            }

            Commands.Checkout(gitRepo, branch);
        }

        public async Task CreateCommitStatus(string project, string repository, string workdirPath, bool isSuccess, string description, string contextName, string contextGenre, string targetUrl)
        {
            var gitClient = Connection.GetClient<GitHttpClient>();
            var repo = await gitClient.GetRepositoryAsync(project, repository);

            using (var gitRepo = new Repository(workdirPath))
            {
                var commitId = gitRepo.Head.Tip.Id.Sha;

                var status = new GitStatus
                {
                    State = isSuccess ? GitStatusState.Succeeded : GitStatusState.Error,
                    Description = description,
                    Context = new GitStatusContext
                    {
                        Name = contextName,
                        Genre = contextGenre
                    },
                    TargetUrl = targetUrl
                };

                await gitClient.CreateCommitStatusAsync(status, commitId, repo.Id);
            }
        }
    }
}
