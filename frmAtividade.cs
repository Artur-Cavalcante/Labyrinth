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
        private string VerificarProximoCaminho(string[,] matrizLabirinto, string localizacaoAtual, string logCaminhosPercorridos, int linhas, int colunas)
        {
            /*
             * Verifica o próximo caminho de acordo com as preferências C > E > D > B
             * A lógica usada para saber o próximo caminho é:
             *  Exemplo usado localização: [4, 5]
             *  1- Cima: [linhaAtual - 1, colunaAtual] => [4, 5] -> [3, 5]
             *  2- Esquerda: [linhaAtual, colunaAtual - 1] => [4, 5] -> [4, 4]
             *  3- Direita: [linhaAtual, colunaAtual + 1] => [4, 5] -> [4, 6]
             *  4- Baixo: [linhaAtual + 1, colunaAtual] => [4, 5] -> [5, 5]
             */

            string proximoCaminho = "";
            int linhaAtual = Convert.ToInt32(Char.GetNumericValue(localizacaoAtual[0]));
            int colunaAtual = Convert.ToInt32(Char.GetNumericValue(localizacaoAtual[1]));

            string caminhoCima = "C[";
            string caminhoEsquerda = "E[";
            string caminhoDireita = "D[";
            string caminhoBaixo = "B[";

            if( !((linhaAtual - 1) < 0) ) //Evita estar fora do range da matriz, antes de analisar
            {
                // 1 - Verifca se CIMA é um próximo caminho 
                if(matrizLabirinto[linhaAtual - 1, colunaAtual] == "0")
                {
                    caminhoCima +=  $"{linhaAtual - 1},{colunaAtual}]-";
                }
            }
            
            if( !((colunaAtual - 1) < 0) )
            {
                // 2 - Verifica se ESQUERDA é um próximo caminho
                if(matrizLabirinto[linhaAtual, colunaAtual - 1] == "0")
                {
                    caminhoEsquerda += $"{linhaAtual},{colunaAtual - 1}]-";
                }
            }
                
            if( !((colunaAtual + 1) > (colunas - 1)) )
            {
                // 3 - Verifica se DIREITA é próximo caminho
                if(matrizLabirinto[linhaAtual, colunaAtual + 1] == "0")
                {
                    caminhoDireita = $"{linhaAtual},{colunaAtual + 1}]-";
                }
            }
                    
            // 4 - Caso nenhum, BAIXO será o próximo caminho
            caminhoBaixo += $"{linhaAtual + 1},{colunaAtual}]-";
            
            //Análise caminho não percorrido
             this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, "Analise -> " + this.AnalisarSeCaminhoPercorrido(logCaminhosPercorridos, caminhoDireita));
                }));
            proximoCaminho = caminhoDireita;
            return proximoCaminho;
        }

        private bool AnalisarSeCaminhoPercorrido(string logCaminhosPercorridos, string caminhoAnalisar)
        {
            
            string [] logArrayCaminhosPercorridos = logCaminhosPercorridos.Split("-");
            foreach(string caminhoPercorrido in logArrayCaminhosPercorridos)
            {
                if(caminhoAnalisar == caminhoPercorrido)
                {
                    return true;
                }
            }

            return false;
        }
        //Fim das Funções Utilizadas na fase de Percorrer


        private void CodigoAtividade(string filePath)
        {
            /*
             Lógica aqui!

            Dividido em 3 partes
                - Preparação
                    -        
                - Percorrendo
                    -
                - Finalizando
                    -
            -boas praticas
            -frase
            */

            //Preparação
            string textoEntrada = File.ReadAllText(filePath);
            string textoLimpo = this.RemoverEspacos(textoEntrada);
           
            int linhas = Convert.ToInt32(Char.GetNumericValue(textoLimpo[0]));
            int colunas = Convert.ToInt32(Char.GetNumericValue(textoLimpo[1]));

            string [,] matrizLabirinto =  new string[ linhas, colunas ];
            matrizLabirinto = this.InserirDadosMatriz(matrizLabirinto, textoLimpo, linhas, colunas);
            
            string localizacaoX = this.EncontrarX(matrizLabirinto, linhas, colunas);             
            string localizacaoSaida = this.EncontrarSaida(matrizLabirinto, linhas, colunas);
            //Fim Preparação
            

            //Percorrendo
                                           // logCaminhoPercorrido.Split("-");
            string logCaminhosPercorridos = $"O[{localizacaoX[0]},{localizacaoX[1]}]-";
            
            //while(localizacaoX != localizacaoSaida)
            //{
                string proximoCaminho = this.VerificarProximoCaminho(matrizLabirinto, localizacaoX, logCaminhosPercorridos,linhas, colunas);
                localizacaoX = $"{proximoCaminho[2]}{proximoCaminho[4]}";
                
                logCaminhosPercorridos += $"{proximoCaminho}";
            //};

            this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, "Logcaminho -> " + logCaminhosPercorridos);
                }));

            

            //Fim Percorrendo


            //Finalizando
            //Fim Finalizando


        }
    }

}
