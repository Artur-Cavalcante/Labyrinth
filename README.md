### TestAtosCapital

Teste prático TI 04/07/2020 – O desafio do labirinto
Será dada uma entrada em arquivo texto, onde na primeira linha contém as dimensões do labirinto
(Linhas Colunas) e nas demais linhas o labirinto em si, em que:
- 1 indica uma parede (isto é, não pode seguir neste ponto da matriz)
- 0 indica um caminho possível de se trafegar
- X é o ponto de partida (não necessariamente é um canto do mapa)
O objetivo é encontrar a única saída, sem "andar pelas paredes" e seguindo a seguinte ordem de
prioridade (quando puder se deslocar):
1) Ir para cima (C)
2) Ir para a esquerda (E)
3) Ir para a direita (D)
4) Ir para baixo (B)
Caso se alcance um ponto em que não é possível se movimentar e/ou não tenham mais posições
que ainda não foram visitadas, deve-se retornar usando o mesmo caminho utilizado até este ponto
“sem-saída” até o último ponto onde teve mais de uma posição possível de movimento. A ordem
de movimento só é utilizada quando há mais de uma posição possível de movimento para posições
ainda não visitadas.
O desafio é elaborar um código-fonte dentro do projeto Template dentro da função
CodigoAtividade() que seja capaz de:
1) ler o arquivo texto de entrada
2) identificar a dimensão da matriz do labirinto, em que o primeiro número indica o número de
linhas e o segundo número indica o número de colunas (é separado por espaço)
3) identificar a posição de origem (ponto O localizado dentro da matriz). A posição “aumenta
de valor” lendo de cima para baixo e/ou da esquerda para a direita. A posição na
extremidade superior esquerda é a [1, 1] (linha 1 coluna 1) e a posição na extremidade
inferior direita é a que representa o número de linhas e o número de colunas [L, C] (exemplo,
se tem 4 linhas e 5 colunas, esta extremidade em questão é a [4, 5])
4) a partir do ponto de origem se deslocar (seguindo a ordem de prioridade de deslocamento)
e encontrar a única saída (que se encontra no ponto 0 localizado em uma extremidade da
matriz)
5) ao encontrar a saída gerar um arquivo texto de saída (na mesma pasta onde está o arquivo
de entrada, só que com outro nome de arquivo. ex: entrada.txt é arquivo de entrada então
o arquivo de saída pode ser saída-entrada.txt) contendo cada passo do trajeto, onde cada
linha indica a direção e posição destinada. A primeira linha do arquivo de saída deve estar
com O (origem) seguido da posição inicial
