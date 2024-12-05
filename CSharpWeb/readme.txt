

- primeiro get
	- construir a estrutura global de nodes html, evento build, cachear em produção
	- chamar o evento render da estrutura
	- retornar plain html
	apos
	- carregar o ws e conectar

- proximas iterações
	- criar uma lista de commandos para serem executados no client
		- commandos como mudar innerContent, adicionar child, remover child, etc
	- blocos htmls podem ser de duas form
		opção 1
			- htmls prontos para serem atachados
			pos = menor scripts js, simplicidade e facilidade para resolver tudo no render
			contra = possivel blocos html maiores
		opção 2
			- estrutura binaria template para ser renderizado no client
			pos = blocos htmls menores
			contra = maior complexidade, maior quantidade de scripts js, possivel complicações em
				eventos mais complexos
	- retornar a lista de commandos para ser executado no client


node lifecycle
	- build
	- render


for each
if
switch
