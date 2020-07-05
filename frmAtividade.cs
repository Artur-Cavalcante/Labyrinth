using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atividade
{
    public partial class frmAtividade : Form
    {
        public frmAtividade()
        {
            InitializeComponent();
        }



        private void btnRun_Click(object sender, EventArgs e)
        {
            if (txtArquivo.Text.Trim().Equals(""))
            {
                MessageBox.Show(this, "Caminho do arquivo deve ser informado");
                txtArquivo.Focus();
                return;
            }

            if (!File.Exists(txtArquivo.Text.Trim()))
            {
                MessageBox.Show(this, "Arquivo inexistente!");
                txtArquivo.Focus();
                return;
            }

            Thread thread = new Thread(() => ExecutaAtividade(txtArquivo.Text.Trim()));
            thread.Name = "Atividade - Run";
            thread.Start();
        }


        private void ExecutaAtividade(string filePath)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                txtArquivo.Enabled = false;
                btnRun.Enabled = false;
            }));

            try
            {
                CodigoAtividade(filePath);

                this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, "Finalizado!");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, ex.Message);
                }));
            }
            finally
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    txtArquivo.Enabled = true;
                    btnRun.Enabled = true;
                }));
            }
        }

        //Início das Funções Utilizadas na Preparação
        private string RemoverEspacos(string textoEntrada)
        {
            /*
             * Este método é utilizado para remover os espaços e enters que estão no texto da matriz,
             * deixando mais simples para percorrer; 
             */;

            string textoLimpo = textoEntrada.Replace(" ", "");
            textoLimpo = textoLimpo.Replace("\r", "");
            textoLimpo = textoLimpo.Replace("\n", "");

            return textoLimpo;
        }

        private string[,] InserirDadosMatriz(string[,] matrizLabirinto, string textoLimpo, int linhas, int colunas)
        {
            /*
             * Este método recebe a martriz já criada, porém vazia, e insere os dados nela
             * que são fornecidos através do parâmetro textoLimpo, onde contem toda a matriz em ordem,
             * sem espaços. E juntamente com as linhas e colunas, consegue inserir corretamente os dados;
            */

            int indiceTextoMatriz = 2;

            //Percorrendo linha
            for(int indexLinha = 0; indexLinha < linhas; indexLinha++)
            {
                //Percorrendo coluna
                for(int indexColuna = 0; indexColuna < colunas; indexColuna++)
                {
                    matrizLabirinto[indexLinha, indexColuna] = Char.ToString(textoLimpo[indiceTextoMatriz]);

                    indiceTextoMatriz++;
                }
            }
            
            return matrizLabirinto;
        }

        private string EncontrarX(string[,] matrizLabirinto, int linhas, int colunas)
        {
            /*
             * Este método percorre a matriz para encontrar a localização do X,
             * e retorna uma string com a localização, exemplo: "51", linha 5 e coluna 1 
             */
            
            string localizacaoX = "";
            
            //Percorrendo linha
            for(int indexLinha = 0; indexLinha < linhas; indexLinha++)
            {
                //Percorrendo coluna
                for(int indexColuna = 0; indexColuna < colunas; indexColuna++)
                {
                    if(matrizLabirinto[indexLinha, indexColuna] == "X")
                    {
                       localizacaoX = $"{indexLinha}{indexColuna}";
                    }
                }
            }
            
            return localizacaoX;
        }

        private string EncontrarSaida(string[,] matrizLabirinto, int linhas, int colunas)
        {
            /*
             * Semelhante ao método EncontrarSaida, percorrer toda a matriz e retorna a localização
             * da saída;
             * A lógica para encontrar a saída é,
             * 1- Precisa ser 0. 
             * 2- Precisa estar nas extremidades.
             * ou seja: 
             *      Condição 1 - [0, X](Qualquer coluna da primeira linha)
             *      Condição 2 -[Ultima, X](Qualquer coluna da ultima linha)
             *      Condição 3 -[X, 0](Qualquer linha da ultima coluna)
             *      Condição 4 -[X, Ultima](Qualquer linha da ultima coluna)
             */

            
            string localizacaoSaida = "";
            
            //Percorrendo linha
            for(int indexLinha = 0; indexLinha < linhas; indexLinha++)
            {
                //Percorrendo coluna
                for(int indexColuna = 0; indexColuna < colunas; indexColuna++)
                {
                   //Para ser uma saída precisa ser 0
                    if(matrizLabirinto[indexLinha, indexColuna] == "0")
                    {
                       //Caso o indice da LINHA seja 0 (Condição 1) ou seja a ultima LINHA (Condição 2)
                       if((indexLinha == 0) || (indexLinha == (linhas -1)))
                        {
                            localizacaoSaida = $"{indexLinha}{indexColuna}";
                        }else
                        {
                            //Caso o indice da COLUNA seja 0 (Condição 3) ou seja a ultima COLUNA (Condição 4) 
                            if((indexColuna == 0) || (indexColuna == (colunas - 1)))
                            {
                                localizacaoSaida = $"{indexLinha}{indexColuna}";
                            }
                        }
                    }
                }
            }
            
            return localizacaoSaida;
        }
        //Fim das Funções Utilizadas na Preparação


        //Início das Funções Utilizadas na fase de Percorrer
        private int CalcularQuantidadePossibilidades(string caminhoCima, string caminhoEsquerda, string caminhoDireita, string caminhoBaixo)
        {
            /*
             * Calcula de acordo com a quantidade de possibilidades de 'próximos caminhos' no caminho atual.
             * Onde posteriormente é necessário saber para poder descobrir o ultimo caminho com mais possiveis caminhos.
             */

            int contadorPossibilidades = 0;

            if(caminhoCima != "")
            {
                contadorPossibilidades++;
            }

            if(caminhoEsquerda != "")
            {
                contadorPossibilidades++;
            }

            if(caminhoDireita != "")
            {
                contadorPossibilidades++;
            }

            if(caminhoBaixo != "")
            {
                contadorPossibilidades++;
            }

            return contadorPossibilidades;
        }

        private void ModuloValores(ref int numero1, ref int numero2, ref int numero3, ref int numero4)
        {
            /*
             * Esta função é simples, apenas verifica os números se são menores que zero e retorna o módulo deles.
             */
            if(numero1 < 0)
            {
                numero1 = numero1 * (-1);
            }

            if(numero2 < 0)
            {
                numero2 = numero2 * (-1);
            }
            
            if(numero3 < 0)
            {
                numero3 = numero3 * (-1);
            }

            if(numero4 < 0)
            {
                numero4 = numero4 * (-1);
            }

        }

        private string VerificarDirecaoCaminhoDePara(string caminhoAtual, string proximoCaminho)
        {
            /*
             * Verifica qual direção de um caminho para o outro. Diferente da lógica que é usado na função VerificarProximoCaminho,
             * aqui só preciso saber a direção mesmo B,C,D ou E.
             * Seguindo a lógica que, se a LINHA do caminho que você quer ir é maior que o atual
             * é necessário ir para baixo, caso seja menor é para cima. Se as LINHA do caminho que você quer ir é igual ao atual
             * então, é na mesma linha porém, colunas diferentes, dai é analisado as colunas. Se a coluna do que você quer ir, é maior
             * que a atual então é necessário ir para direita, se for menor é necessário ir para esquerda.
             */

            int linhaProximoCaminho = Convert.ToInt32(Char.GetNumericValue(proximoCaminho[2]));
            int colunaProximoCaminho = Convert.ToInt32(Char.GetNumericValue(proximoCaminho[4]));

            int linhaCaminhoAtual = Convert.ToInt32(Char.GetNumericValue(caminhoAtual[2]));
            int colunaCaminhoAtual = Convert.ToInt32(Char.GetNumericValue(caminhoAtual[4]));

            string direcao = ""; 

            if(linhaProximoCaminho > linhaCaminhoAtual)
            {
                direcao = "B";
            }else
            {
                if(linhaProximoCaminho == linhaCaminhoAtual)
                {
                    if(colunaProximoCaminho > colunaCaminhoAtual)
                    {
                        direcao = "D";
                    }else
                    {
                        direcao = "E";
                    }
                }else
                {
                    direcao = "C";
                }
            }

            return direcao;

        }

        private string RetornandoUltimoCaminho(ref string logCaminhosPercorridosAuxiliar)
        {
            /*
             * Está função utiliza o log de caminhos percorrido auxiliar para poder ir voltando uma casa anterior,
             * então ele localiza a casa anterior, e aponta como próximo caminho. Ápós isso, zera o auxiliar, reconstroi
             * o log auxiliar só que sem a localização atual. Diferentemente do logCaminhoPercorridos, que tem TODOS os 
             * caminhos já feitos, e posteriormente que é usado para ser gravado no arquivo.
             */
		    string []logArrayCaminhosPercorridosAuxiliar = logCaminhosPercorridosAuxiliar.Split('-');

            string caminhoAtual = logArrayCaminhosPercorridosAuxiliar[logArrayCaminhosPercorridosAuxiliar.Length - 2];
            string proximoCaminho = logArrayCaminhosPercorridosAuxiliar[logArrayCaminhosPercorridosAuxiliar.Length - 3];
            
            char linhaProximoCaminho = proximoCaminho[2];
            char colunaProximoCaminho = proximoCaminho[4];
            
            proximoCaminho = $"{VerificarDirecaoCaminhoDePara(caminhoAtual, proximoCaminho)}[{linhaProximoCaminho},{colunaProximoCaminho}]";

            //Zerando log auxiliar...
            logCaminhosPercorridosAuxiliar = ""; 

            for(int index = 0; index < (logArrayCaminhosPercorridosAuxiliar.Length - 2) ;index++)
            {
                logCaminhosPercorridosAuxiliar +=  $"{logArrayCaminhosPercorridosAuxiliar[index]}-";
            }

            return proximoCaminho;
        }

        private bool AnalisarSeCaminhoJaPercorrido(string logCaminhosPercorridos, string caminhoAnalisar)
        {
            /*
             * Esse método analisa se aquele caminho já foi percorrido alguma vez, e retorna true caso sim, e false caso não
             */

		    string []logArrayCaminhosPercorridos = logCaminhosPercorridos.Split('-');
            caminhoAnalisar = $"{caminhoAnalisar[2]}{caminhoAnalisar[4]}";

            foreach(string caminhoPercorrido in logArrayCaminhosPercorridos)
            {
                //Evita fora de range quando for ocorrer o split
                if(caminhoPercorrido != "")
                {
                    string caminhoPercorridoAnalisar = $"{caminhoPercorrido[2]}{caminhoPercorrido[4]}"; 
                    if(caminhoAnalisar == caminhoPercorridoAnalisar)
                    {
                        return true;
                    }else
                    {
                        continue;
                    }
                }
            }

            return false;
        }

        private string VerificarProximoCaminho(string[,] matrizLabirinto, string localizacaoAtual, string localizacaoOrigem, string logCaminhosPercorridos, int linhas, int colunas, ref string[] ultimoCaminhoMaisPossibilidades, ref string logCaminhosPercorridosAuxiliar)
        {
            /*
             *  Verifica o próximo caminho de acordo com as preferências C > E > D > B
             *  A lógica usada para saber o próximo caminho é:
             *  Primeiro verificamos quais caminho estão possíveis. Para isso, é analisado se ele não está
             *  fora do range da linhas ou coluna da matriz. Após isso, caso seja o caminho para cima,
             *  é só analisar linha atual -1, e mesma coluna. Para esquerda linha atual, porém coluna atual -1.
             *  Para direita linha atual, coluna atual + 1. E por fim, para baixo, linha atual + 1, e coluna atual.
             *  
             *  Exemplo usado localização: [4, 5]
             *  1- Cima: [linhaAtual - 1, colunaAtual] => [4, 5] -> [3, 5]
             *  2- Esquerda: [linhaAtual, colunaAtual - 1] => [4, 5] -> [4, 4]
             *  3- Direita: [linhaAtual, colunaAtual + 1] => [4, 5] -> [4, 6]
             *  4- Baixo: [linhaAtual + 1, colunaAtual] => [4, 5] -> [5, 5]
             *
             *  Com isso em mente, é só verifica se os valores desses caminhos são 0, que é o caso de não ser uma parede(1).
             *  Guardando os caminhos corretos e possíveis é analisado, de forma inversa, se o caminho possível já foi percorrido antes.
             *  É analisado de forma inversa por causa da prioridade C > E > D > B, então é verificado inversamente e sobreposto por último
             *  o de maior prioridade.
             *  Depois de descobrir se todos esses caminhos já foram percorridos antes, calculamos o número de caminhos possíveis
             *  naquela localização atual, isso para posteriormente quando ficarmos sem saída, sabermos onde foi o último caminho com
             *  mais caminhos possíveis.
             * 
             */

            string proximoCaminho = "";
            int quantidadePossibilidadesAtual = 0;

            int linhaAtual = Convert.ToInt32(Char.GetNumericValue(localizacaoAtual[0]));
            int colunaAtual = Convert.ToInt32(Char.GetNumericValue(localizacaoAtual[1]));

            string caminhoCima = "";
            string caminhoEsquerda = "";
            string caminhoDireita = "";
            string caminhoBaixo = "";

            if( !((linhaAtual - 1) < 0) ) //Evita estar fora do range da matriz, antes de analisar
            {
                // 1 - Verifca se CIMA é um próximo caminho 
                if(matrizLabirinto[linhaAtual - 1, colunaAtual] == "0")
                {
                    caminhoCima +=  $"C[{linhaAtual - 1},{colunaAtual}]";
                }
            }
            
            if( !((colunaAtual - 1) < 0) )
            {
                // 2 - Verifica se ESQUERDA é um próximo caminho
                if(matrizLabirinto[linhaAtual, colunaAtual - 1] == "0")
                {
                    caminhoEsquerda += $"E[{linhaAtual},{colunaAtual - 1}]";
                }
            }
                
            if( !((colunaAtual + 1) > (colunas - 1)) )
            {
                // 3 - Verifica se DIREITA é próximo caminho
                if(matrizLabirinto[linhaAtual, colunaAtual + 1] == "0")
                {
                    caminhoDireita += $"D[{linhaAtual},{colunaAtual + 1}]";
                }
            }
                    
            if( !((linhaAtual + 1) > (colunas - 1)) )
            {
                // 4 - Verifica se BAIXO é próximo caminho
                if(matrizLabirinto[linhaAtual + 1, colunaAtual] == "0")
                {
                    caminhoBaixo += $"B[{linhaAtual + 1},{colunaAtual}]";
                }
            }
            
            //Análise se caminho já percorrido;
            if(caminhoBaixo != "")
            {
                if( !(this.AnalisarSeCaminhoJaPercorrido(logCaminhosPercorridos, caminhoBaixo)) )
                {
                    proximoCaminho = caminhoBaixo;
                }
                
            }

             if(caminhoDireita != "")
            {
                if( !(this.AnalisarSeCaminhoJaPercorrido(logCaminhosPercorridos, caminhoDireita)) )
                {
                    proximoCaminho = caminhoDireita;
                }
            }
            
             if(caminhoEsquerda != "")
            {
                if( !(this.AnalisarSeCaminhoJaPercorrido(logCaminhosPercorridos, caminhoEsquerda)) )
                {
                    proximoCaminho = caminhoEsquerda;
                }

            }
             
            if(caminhoCima != "")
            {
                if( !(this.AnalisarSeCaminhoJaPercorrido(logCaminhosPercorridos, caminhoCima)) )
                {
                    proximoCaminho = caminhoCima;
                }
            }

            quantidadePossibilidadesAtual = CalcularQuantidadePossibilidades(caminhoCima, caminhoEsquerda, caminhoDireita, caminhoBaixo);

            if($"{localizacaoAtual[0]}{localizacaoAtual[1]}" != $"{localizacaoOrigem[0]}{localizacaoOrigem[1]}")
            {
                if(quantidadePossibilidadesAtual > Convert.ToInt32(ultimoCaminhoMaisPossibilidades[1]))
                {
                    ultimoCaminhoMaisPossibilidades[0] = $"{localizacaoAtual[0]}{localizacaoAtual[1]}";
                    ultimoCaminhoMaisPossibilidades[1] = $"{quantidadePossibilidadesAtual}";
                }else
                {
                    if(quantidadePossibilidadesAtual == Convert.ToInt32(ultimoCaminhoMaisPossibilidades[1]))
                    {
                        ultimoCaminhoMaisPossibilidades[0] = $"{localizacaoAtual[0]}{localizacaoAtual[1]}";
                        ultimoCaminhoMaisPossibilidades[1] = $"{quantidadePossibilidadesAtual}";
                    }
                }
            }

            //Agora, se faltar caminho não percorrido, e mesmo assim o unico caminho já for percorrido, a única forma é ir
            //para o ultimo caminho com mais possibilidades, porém, voltando de casa em casa.
            if(proximoCaminho == "")
            {
                proximoCaminho = this.RetornandoUltimoCaminho(ref logCaminhosPercorridosAuxiliar);
                return proximoCaminho;
            }else
            {
                logCaminhosPercorridosAuxiliar += $"{proximoCaminho}-";
                return proximoCaminho;
                
            }
        }
        //Fim das Funções Utilizadas na fase de Percorrer


        //Funções Utilizadas na fase de finalização
        private string[] AlterarVisualizacaoLog(string logCaminhosPercorridos)
        {
            /*
             * Tendo em vista que na programação nos começamos um array do 0 e não do 1, precisarei dessa
             * função para poder passar o log de por exemplo O[0,3] para O[1, 4], para que fique visualmente
             * mais simples de uma pessoa ler o log da matriz. Nada aqui alterará no caminho percorrido, apenas
             * na exibição final do log
             */
            int contador = 0;
		    string []logArrayCaminhosPercorridos = logCaminhosPercorridos.Split('-');
            foreach(string caminhoPercorrido in logArrayCaminhosPercorridos)
            {
                //Evita fora de range quando for ocorrer o split
                if(caminhoPercorrido != "")
                {
                    int primeiroElemento = Convert.ToInt32(Char.GetNumericValue(caminhoPercorrido[2]));
                    int segundoElemento =  Convert.ToInt32(Char.GetNumericValue(caminhoPercorrido[4]));
                    logArrayCaminhosPercorridos.SetValue($"{caminhoPercorrido[0]} [{primeiroElemento + 1}, {segundoElemento + 1}]", contador);
                }
                contador++;
            }

            return logArrayCaminhosPercorridos;
        }

        private string CriarNomeArquivoSaida(string filePath, string[] logArrayCaminhosPercorridos)
        {
            /*
             * Este método pega o nome do arquivo e retorna o novo nome do arquivo de saida;
             */
            
            //split por \, os caminhos
            string[] filePathReparticionado = filePath.Split('\\');
            string nomeArquivo = filePathReparticionado[filePathReparticionado.Length - 1];
            string nomeArquivoSaida = $"saida-{nomeArquivo}";

            return nomeArquivoSaida;
        }

        private string PegarCaminhoArquivo(string filePath)
        {
            /*
             * Este método pega o todo caminho do arquivo, e retorna so o caminho do folder;
             */
            string[] filePathReparticionado = filePath.Split('\\');
            string caminhoArquivo = "";
            int contador = 0;
            int ultimo = filePathReparticionado.Length - 1;

            foreach(string caminho in filePathReparticionado)
            {
                if(contador == ultimo)
                {
                    break;
                }else
                {
                    caminhoArquivo += caminho + "\\";
                }

                contador++;
            }

            return caminhoArquivo;
        }

        private void EscreverArquivoSaida(string nomeArquivoSaida, string caminhoArquivoSaida, string[] logArrayCaminhosPercorridos)
        {
           /*
            * Este método cria o arquivo a partir do nome dado, e insere o log dos caminhos percorridos
            */ 
            string logCaminhosPercorridos = "";
            foreach(string caminho in logArrayCaminhosPercorridos)
            {
                logCaminhosPercorridos += $"{caminho}\n";
            }

            File.WriteAllText($"{caminhoArquivoSaida}" + $"{nomeArquivoSaida}", logCaminhosPercorridos);
        }
        //Fim das Funções Utilizadas na fase de finalização


        private void CodigoAtividade(string filePath)
        {
            /*
            - José Artur Cavalcante Paulino
            - Data: 05/07/2020 - Domingo
            - Prefácio:
                As funções que foram criadas para dividir o código, estão fora desta função 'CodigoAtividade', estão acima dela. 
                Cada função utilizada tem uma explicação do que ela faz no cabeçalho.
                O código foi dividido em 3 partes:
            
                    - Preparação (Funções localizadas linha 80 -> 190)
                        - Inicialmente na fase de preparação o arquivo com o labirinto é lido, após ser lido é retirado espaços e enters.
                          Com o texto já limpo, retiro o numero de linhas e colunas, para poder criar uma matriz vazia com a dimensão correta.
                          Tendo criado a matriz, agora é inserido os dados nela, para facilitar o uso no algoritmo, em vez de utilizar uma grande
                          string com toda matriz.
                          Após criada, é a hora de procurar a localização do X, ou seja, A origem(A variável origem é utilizada depois), e a localização
                          da saída.

                    - Percorrendo (Funções localizadas linha 193 -> 492)
                        - Após a fase de preparação, entramos em um while para verificar o proximo caminho.
                          Com o próximo caminho, passamos para a localização do X, e guardamos no log para exibir depois o trajeto
                          percorrido.
                        - A lógica principal de verificar o próximo caminho está no cabeçalho da função próximo caminho.

                    - Finalizando (Funções linha 495 -> 574)
                        - No fim, depois de preparar, e percorrer a matriz, é hora de guardar o log do caminho percorrido no arquivo de texto.
                          Então, a partir do método AlterarVisualizacaoLog passamos o log corrigido para o array. Após isso, criamos o nome do arquivo
                          de saída, e também pegamos o caminho dele, que é o mesmo do de entrada, porém com o nome 'saida-' na frente do arquivo de saída.
                          E enfim, é escrito o log no arquivo.
            */

            //1-Preparação
            string textoEntrada = File.ReadAllText(filePath);
            string textoLimpo = this.RemoverEspacos(textoEntrada);
           
            int linhas = Convert.ToInt32(Char.GetNumericValue(textoLimpo[0]));
            int colunas = Convert.ToInt32(Char.GetNumericValue(textoLimpo[1]));

            string [,] matrizLabirinto =  new string[ linhas, colunas ];
            matrizLabirinto = this.InserirDadosMatriz(matrizLabirinto, textoLimpo, linhas, colunas);
            
            string localizacaoX = this.EncontrarX(matrizLabirinto, linhas, colunas);             
            string localizacaoOrigem = localizacaoX;
            string localizacaoSaida = this.EncontrarSaida(matrizLabirinto, linhas, colunas);
            //1-Fim Preparação
            

            //2-Percorrendo

            // Depois será utilizado como logCaminhoPercorrido.Split("-");
            string logCaminhosPercorridos = $"O[{localizacaoX[0]},{localizacaoX[1]}]-";
            //log de caminho apenas para auxliar quando precisar voltar
            string logCaminhosPercorridosAuxiliar = $"O[{localizacaoX[0]},{localizacaoX[1]}]-";

            //Será [ulimoCaminhoMaisPossibilidades, numeroDePossibilidades] exemplo [32, 2] 
            string[] ultimoCaminhoMaisPossibilidades = {"", "0"};
            
            while(localizacaoX != localizacaoSaida)
            {
                
                string proximoCaminho = this.VerificarProximoCaminho(matrizLabirinto, localizacaoX, localizacaoOrigem, logCaminhosPercorridos, linhas, colunas, ref ultimoCaminhoMaisPossibilidades, ref logCaminhosPercorridosAuxiliar);
                localizacaoX = $"{proximoCaminho[2]}{proximoCaminho[4]}";
                
                logCaminhosPercorridos += $"{proximoCaminho}-";
            };
            //2-Fim Percorrendo


            //3-Finalizando
            string [] logArrayCaminhosPercorridos = this.AlterarVisualizacaoLog(logCaminhosPercorridos);
            string nomeArquivoSaida = this.CriarNomeArquivoSaida(filePath, logArrayCaminhosPercorridos);
            string caminhoArquivoSaida = this.PegarCaminhoArquivo(filePath);
            this.EscreverArquivoSaida(nomeArquivoSaida, caminhoArquivoSaida,logArrayCaminhosPercorridos);            
            //3-Fim Finalizando
        }
    }

}
