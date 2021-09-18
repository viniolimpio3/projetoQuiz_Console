using System;
using System.IO;

namespace projetoQuiz_n2_Console {
    class Program {

        const int QTD_ALTERNATIVAS = 4;
        const int QTD_TEMAS = 100;//qtd maxima
        const int QTD_PERGUNTAS = 1000;//qtd maxima
        const string TEXT_FILE = "QUIZ.txt";

        struct Tema {
            public string key;
            public Pergunta[] perguntas;
            public int qtdPerguntas;
        }


        /**
         * Estrutura das Perguntas em si
         */
        struct Pergunta {
            public string pergunta;
            public string[] alternativas;
            public int indexCorreto; //indice do array 'alternativas' que corresponde a alternativa correta!
        }


        /// <summary>
        /// Lê arquivo QUIZ.txt e insere as perguntas dentro das structs
        /// </summary>
        static Tema[] lePerguntas() {
                        
            string[] linhas = File.ReadAllLines(TEXT_FILE);

            Tema[] temas = new Tema[QTD_TEMAS];

            int qtdTemasUsados = 0;

            foreach(string linha in linhas) {
                string[] vetorLinha = linha.Split("|");

                qtdTemasUsados = getQtdTemasCadastrados(temas);

                Pergunta pergunta = new Pergunta();

                int index = retornaIndiceTemaCorrespondente(temas, qtdTemasUsados, vetorLinha[1]);

                int qtdPerguntasDoTema = getQtdPerguntasCadastradas(temas[index].perguntas);

                if(qtdPerguntasDoTema == 0)
                    temas[index].perguntas = new Pergunta[QTD_PERGUNTAS];

                pergunta.pergunta = vetorLinha[0];
                pergunta.alternativas = new string[] {vetorLinha[3], vetorLinha[4], vetorLinha[5], vetorLinha[6]};
                pergunta.indexCorreto = getIndexAlternativaCorreta(pergunta.alternativas, vetorLinha[2]);

                //insere na última posição de pergunta 
                temas[index].key = vetorLinha[1];
                temas[index].perguntas[qtdPerguntasDoTema] = pergunta;
                temas[index].qtdPerguntas = qtdPerguntasDoTema == 0 ? 1 : qtdPerguntasDoTema; // não pode existir zero qtd de perguntas de um tema
            }

            return temas;

        }

        /**
         * Busca qual o índice dentro dos array de alternativas corresponde a alternativa correta
         */
        static int getIndexAlternativaCorreta(string[] alternativas, string alternativaCorreta) {
            int indexCorreto = 0;
            for(int i=0; i < alternativas.Length; i++) {
                if (alternativas[i] == alternativaCorreta) {
                    indexCorreto = i;
                    break;
                }

            }

            return indexCorreto;

        }

        /**
         * Retorna a Quantidade de Temas Cadastrados efetivamente
         */
        static int getQtdTemasCadastrados(Tema[] temas) {
            int qtd = 0;

            foreach(Tema tema in temas) {

                if (tema.key == null)
                    break;

                qtd++;
            }

            return qtd;
        }

        /**
         * Retorna a Quantidade de Perguntas do Tema Cadastrados efetivamente
         */
        static int getQtdPerguntasCadastradas(Pergunta[] perguntas) {
            int qtd = 0;

            if (perguntas == null)
                return qtd; //0
            
            foreach (Pergunta pergunta in perguntas) {

                if (pergunta.pergunta == null)
                    break;

                qtd++;
            }

            return qtd;
        }

        /**
         * Percorre um vetor de temas e verifica se a key já existe na estrutura 
         * ou retorna o último indice de array utilizado
         */
        static int retornaIndiceTemaCorrespondente(Tema[] temas, int qtdTemas, string key) {

            //se for igual à zero, não soma 1
            int lastIndex = qtdTemas;

            for(int i=0; i < qtdTemas; i++) {
                if (temas[i].key == key) {
                    lastIndex = i;
                    break;
                }
            }
            
            return lastIndex;
        }

        static void Main(string[] args) {
            Tema[] temas = lePerguntas();


            Console.WriteLine("Hello World!");
        }


    }
}
