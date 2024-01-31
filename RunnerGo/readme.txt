

- fazer limpeza de ScriptContent q não é mais usado
- visualizar types input e output nos scripts
- impelmentar modificação de type nas run
- criar o jobschedule
- melhorar o componente table
- fazer filtos de tabela
- criar o component de navegação lateral esquerdo

root/
	users
		/eu
		/outro
	apps
		/nexlog/




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
			







agent
	vers
		verions.json
	src
	ws
		workspace-table.json
		0
		1
		2
		3



- recebe o pacote

- descompata na pasta vers\vX

- sinaliza o evento
- desconecta o atual
- chama o proximo

	- espera o agent parar observando o process
	- copia a root do agent para o _bkp
	- copia os novos arquivos para a root
	- da start no serviço




