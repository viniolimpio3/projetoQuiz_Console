using System;
using System.IO;
using System.Threading;

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
            public int respostaUsuario;
        }

        static void Main(string[] args) {
            Tema[] temas = lePerguntas();


            Console.WriteLine("Bem vindo ao Jogo QUIZ!!! \\o/\n----------------------------------------------------");

            do {
                int qtdTemas = getQtdTemasCadastrados(temas);

                exibeMenu(temas, qtdTemas);

                int opcao = leOpcaoMenu(qtdTemas);

                //se o retorno for -1, significa que o user digitou esc - logo, sai do programa

                int qtd_perguntas_cadastradas = getQtdPerguntasCadastradas(temas[opcao].perguntas);

                int qtdPerguntasSolicitadas = leInteger($"Digite a quantidade de Perguntas (máx. {qtd_perguntas_cadastradas})", qtd_perguntas_cadastradas);

                Pergunta[] perguntas = new Pergunta[qtdPerguntasSolicitadas];
                perguntas = getRespostas(getPerguntasRandom(qtdPerguntasSolicitadas, temas[opcao].perguntas));

                exibeResultado(perguntas);

                Console.WriteLine("\nPressione qualquer tecla para prosseguir ou ESC para encerrar o programa :D");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;



            } while (true);

            Console.WriteLine("Saindo.... :(");
            Thread.Sleep(2000); // pausa de 2 segundos
        }

        static void exibeResultado(Pergunta[] perguntas) {
            int qtd_respostas_certas = getQtdRespostasCertas(perguntas);
            int qtd_respostas_erradas = getQtdPerguntasCadastradas(perguntas) - qtd_respostas_certas;

            int countP = 0;
            foreach(Pergunta p in perguntas) {
                Console.WriteLine("\n---------------------------------------------------");
                Console.WriteLine($"Pergunta {++countP}: {p.pergunta}");
                Console.WriteLine($"Resposta Correta: {p.alternativas[p.indexCorreto]}");

                
                if (p.indexCorreto == p.respostaUsuario)
                    exibeTextoSuccess($"Sua resposta: {p.alternativas[p.respostaUsuario]}\nVocê digitou a resposta Certa!", false);
                else
                    exibeTextoAlerta($"Sua Resposta: {p.alternativas[p.respostaUsuario]}\nOh no! Resposta Incorreta :/", false);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n---------------------------------------------------");
            Console.WriteLine($"Quantidade de Acertos: {qtd_respostas_certas}");
            Console.WriteLine($"Quantidade de Erros: {qtd_respostas_erradas}");

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        static int getQtdRespostasCertas(Pergunta[] perguntas) {
            int qtd = 0;
            foreach (Pergunta p in perguntas) {
                if (p.respostaUsuario == p.indexCorreto) 
                    qtd++;
            }

            return qtd;
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
                temas[index].qtdPerguntas = qtdPerguntasDoTema == 0 ? 1 : qtdPerguntasDoTema; // não existe zero qtd de perguntas de um tema!
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
                return 0;
            
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
                if (limpaString(temas[i].key) == limpaString(key) ) {
                    lastIndex = i;
                    break;
                }
            }
            
            return lastIndex;
        }

        /// <summary>
        /// Limpa String
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string</returns>
        static string limpaString(string str) {

            str = str.Trim();

            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                str = str.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return str;
        }


        static void exibeMenu(Tema[] temas, int qtdTemas) {
            Console.WriteLine("Digite o número para selecionar o tema do seu Jogo:\n");

            for (int i = 0; i < qtdTemas; i++) 
                Console.WriteLine($"[{i + 1}] - {temas[i].key}");
        }

        /// <summary>
        /// Lê numero Inteiro Maior que zero tratando exceptions
        /// </summary>
        /// <param name="msg"> Text Escrito no console </param>
        /// <returns> int </returns>
        static int leInteger(String msg, int valorMax = -1) {
            do {
                try {
                    Console.WriteLine(msg);

                    int n = Convert.ToInt32(Console.ReadLine());

                    //Validações
                    if(n <= 0) {
                        exibeTextoAlerta("Digite um número maior que Zero!");
                        continue;
                    }else if(valorMax != -1 && n > valorMax) {
                        exibeTextoAlerta($"Digite um número Menor que {valorMax}!");
                        continue;
                    }

                    return n;
                } catch {
                    exibeTextoAlerta("Digite um número Inteiro Válido!");
                }
            } while (true);
        }

        /**
         * Lê a opção do Menu, fazendo as tratativas necessárias!
         */
        static int leOpcaoMenu(int qtdTemas) {
            do {
                try {
                    Console.WriteLine("Digite o tema das perguntas:");

                    int opcao = Convert.ToInt32(Console.ReadLine());

                    if (opcao > qtdTemas || opcao <= 0) {
                        exibeTextoAlerta("Digite uma opção de tema válido!");
                        continue;
                    }
                    
                    return opcao - 1;
                } catch {
                    exibeTextoAlerta("Opção Inválida!");
                }
            } while (true);
        }

        static void exibeTextoAlerta(string text, bool breakLine = true) {
            Console.ForegroundColor = ConsoleColor.Red;
            if (breakLine) Console.WriteLine("\n");
            Console.WriteLine(text);
            if (breakLine) Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /**
         * Exibe textos em verde!
         * (ps. não é verde do palmeiras professor, não desconte pontos! rsrs)
         */
        static void exibeTextoSuccess(string text, bool breakLine = true) {
            Console.ForegroundColor = ConsoleColor.Green;
            if(breakLine) Console.WriteLine("\n");
            Console.WriteLine(text);
            if (breakLine) Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /**
         * Retorna se um valor está dentro do vetor de int
         */
        static bool estaNoVetor_Int(int[] vetor, int value) {
            bool estaNoVetor = false;
            foreach (int item in vetor)
                if (item == value) {
                    estaNoVetor = true;
                    break;
                }

            return estaNoVetor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qtdSolicitadas"> Quantidade que usuário digitou de perguntas à serem exibidas </param>
        /// <param name="todasPerguntas"> Todas as perguntas do tema </param>
        /// <returns> O vetor com perguntas Aleatórias </returns>
        static Pergunta[] getPerguntasRandom(int qtdSolicitadas, Pergunta[] todasPerguntas) {

            int[] indicesUsados = new int[qtdSolicitadas];
            //iniciando valores com -1 para poder achar com indexOf
            for (int a = 0; a < qtdSolicitadas; a++)
                indicesUsados[a] = -1;

            int qtdTotal = getQtdPerguntasCadastradas(todasPerguntas);

            Pergunta[] perguntas = new Pergunta[qtdSolicitadas];

            Random r = new Random();
            for (int i = 0; i < qtdSolicitadas; i++) {
                int randomIndex = r.Next(qtdTotal);

                //se o valor randomico já tiver sido sorteado, refaz a lógica até não estar
                while (estaNoVetor_Int(indicesUsados, randomIndex))
                    randomIndex = r.Next(qtdTotal);

                perguntas[i] = todasPerguntas[randomIndex];

                //o índice do array já foi usado, agora só jogar no array de indices que já foram usados!
                int pos = Array.IndexOf(indicesUsados, -1);
                if (pos != -1)
                    indicesUsados[pos] = randomIndex;

            }

            return perguntas;

        }

        /// <summary>
        /// Exibe as Perguntas fazendo todas as tratativas necessárias e retorna o array de perguntas com as respostas do usuário
        /// </summary>
        /// <param name="perguntas"> Perguntas para serem exibidas </param>
        static Pergunta[] getRespostas(Pergunta[] perguntas) {

            int count = 0;
            for(int a=0; a < getQtdPerguntasCadastradas(perguntas); a++) {

                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine($"Pergunta {++count}: {perguntas[a].pergunta}");

                Console.WriteLine("\nAlternativas:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                for(int i=0; i < 4; i++) 
                    Console.WriteLine($"[{i + 1}] - {perguntas[a].alternativas[i]}");
                
                
                Console.ForegroundColor = ConsoleColor.Gray;

                perguntas[a].respostaUsuario = leInteger("\nDigite o número correspondente à sua Resposta", 4) - 1; //para ajustar com o index!
            }

            return perguntas;

        }
    }
}
