

- criar o visualizador na lateral esquerda
- criar o controlador de atalhos da home


Kernel
	Commands
		

Domain
	- Kernel
	Entities
		Product
	Queries
		Product
			GetProductsQuery

Infrastructure
	- Kernel
	- Domain
	QueryHandlers
		Product
			GetProductsQueryHandler

Application
	- Kernel
	- Domain
	Handlers
		Product
			GetProductsCommandHandler
	Commands
		Product
			GetProductsCommand
			GetProductsResult

	
WebUI
	- Kernel
	- Infrastructure
	- Application

var result = await _kernel.Execute(new GetProductsCommand());


Logged
/me = UserHome
/me/apps

/user/[name] = view user
/app/[name] = navigate
[else] = navegar para o node



agent
	vs
		v0
	src
	ws
		workspace-table.json
		0
		1
		2
		3
	_update
		v1
		restart

task.tag
	nexlog
configuration
	nexlog,demo

Runner.Task.Hosting
	Entities
		Task
			- id
			- name
		TaskConfiguration
			- taskId
			- configuration
			- tags
		TaskRunner
			- id
			- machineName
			- instanceName
			- localTags
			- tags
			- enabled
		TaskRunnerHeartbeat
			- taskRunnerId
			- datetime
		TaskInstance
		TaskInstanceLog


- criar uma nova task, implementa a ITask, seta a defaultConfiguration e tags via atributo e pronto

TasksWorker
	- ao iniciar
		buscar todas classes nesse assembly que implementa a ITask
		checar se existem na tabela Task e criar se n existe
	- loop em 1s
		pega as localTags do appsettings.json
		checa atualizando a TaskRunner com essa instancia atualizando o pegando o tags
		atualiza e limpa o TaskRunnerHeartbeat
		junta primeiro localTags com o tags do banco
		
		checa as tasks que pode rodar,
			bate as tags do runner com as tags das tasks, e considera se um tag bater

		busca todas possiveis configurações para as tasks consideradas
			