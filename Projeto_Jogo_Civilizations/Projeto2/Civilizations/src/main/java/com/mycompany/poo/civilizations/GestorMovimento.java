/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.*;
import java.io.*;

/**
 *
 * @author Diogo
 */
public class GestorMovimento implements IAcao{
    private static final int MAX_MOVIMENTO = 5;
    
    private IMapa mapa;
    private static List<AgendaAcao> movimentosPlaneados;
    private Scanner s;

    public GestorMovimento(IMapa mapa) {
        this.mapa = mapa;
        this.s = new Scanner(System.in);
        movimentosPlaneados = new ArrayList<>();
    }   
    /**
    * Grava os movimentos planejados em um arquivo.
    *
    * Este método escreve as ações de movimento em um arquivo especificado, 
    * formatando os dados de cada ação como uma string. Cada linha do arquivo 
    * representa uma ação de movimento.
    *
    * @param caminhoArquivo o caminho do arquivo onde os movimentos serão gravados
    * @throws IOException se ocorrer um erro de entrada/saída durante a gravação
    */
   public static void gravarMovimentos(String caminhoArquivo) throws IOException {
       try (BufferedWriter writer = new BufferedWriter(new FileWriter(caminhoArquivo))) {
           for (AgendaAcao acao : movimentosPlaneados) {
               // Formatar os dados do objeto como uma string
               String linha = acao.getUnidade().toString() + "," + 
                              acao.getUnidade().getPosX() + "," + 
                              acao.getUnidade().getPosY() + "," + 
                              acao.getDestinoX() + "," + 
                              acao.getDestinoY();
               writer.write(linha);
               writer.newLine(); // Nova linha para o próximo objeto
           }
       } catch (IOException e) {
           System.err.println("Erro ao gravar movimentos: " + e.getMessage());
       }
   }

   /**
    * Carrega os movimentos planejados de um arquivo.
    *
    * Este método lê as ações de movimento de um arquivo e as adiciona à lista 
    * de movimentos planejados. Cada linha do arquivo deve conter os dados de 
    * uma ação de movimento formatados corretamente.
    *
    * @param reader o BufferedReader usado para ler os dados do arquivo
    * @param unidades um mapa que relaciona tipos de unidades a suas instâncias
    * @throws IOException se ocorrer um erro de entrada/saída durante a leitura
    */
   public static void carregarMovimentos(BufferedReader reader, Map<String, List<IUnidade>> unidades) throws IOException {
       String linha;
       while ((linha = reader.readLine()) != null) {
           // Dividir a linha nos dados do objeto
           String[] partes = linha.split(",");
           if (partes.length == 5) {
               String tipoUnidade = partes[0];
               int unidadeX = Integer.parseInt(partes[1]);
               int unidadeY = Integer.parseInt(partes[2]);
               int destinoX = Integer.parseInt(partes[3]);
               int destinoY = Integer.parseInt(partes[4]);
               // Buscar a unidade correspondente pelas coordenadas
               for (IUnidade unidade : unidades.get(tipoUnidade)) {
                   if ((unidade.getPosX() == unidadeX) && (unidade.getPosY() == unidadeY)) {
                       movimentosPlaneados.add(new AgendaAcao(unidade, destinoX, destinoY));                            
                   }
               }
           }                   
       }       
   }

   /**
    * Processa a escolha da unidade pelo jogador.
    *
    * Este método solicita ao jogador que escolha uma unidade a partir de uma 
    * lista de opções. O método continua solicitando até que uma escolha válida 
    * seja feita.
    *
    * @param opcao a opção escolhida pelo jogador
    * @return o tipo da unidade selecionada como uma string
    */
   public String processarEscolhaUnidade(int opcao) {  
       Scanner s = new Scanner(System.in);
       do {
           try {
               opcao = s.nextInt();
               switch (opcao) {
                   case 1:
                       return "COLONO";
                   case 2:
                       return "MILITAR";
                   case 3:
                       return "CONSTRUTOR";
                   case 0:
                       break;
                   default:
                       GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                       break;
               } 
           } catch (InputMismatchException e) {
               GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
               s.nextLine(); // Limpa o buffer de entrada inválida
           }

       } while (!((opcao >= 0) && (opcao < 4))); 
       return "";
   }

   /**
    * Move a unidade para a posição de destino, se possível.
    *
    * Este método verifica se a unidade pode ser movida para a posição de destino 
    * especificada. Se o movimento for válido, a unidade é marcada como ocupada 
    * e o movimento é agendado.
    *
    * @param unidade a unidade a mover
    * @param destinoX a coordenada X de destino
    * @param destinoY a coordenada Y de destino
    */
    public void moverUnidade(IUnidade unidade, int destinoX, int destinoY) {
        // Ajustar destinoX para o mapa circular horizontalmente
        destinoX = (destinoX + mapa.getLargura()) % mapa.getLargura();        
        // Verifica se o movimento é válido e move a unidade
        if (podeMoverUnidade(unidade, destinoX, destinoY)) {
            unidade.setOcupada(true);           
            // Remover do terreno atual
            ITerreno terrenoAtual = mapa.getTerreno(unidade.getPosX(), unidade.getPosY());
            terrenoAtual.clearOverlaySimbolo(unidade.getSimbolo());

            movimentosPlaneados.add(new AgendaAcao(unidade, destinoX, destinoY));
            GestorMensagens.mostrarMensagem("Movimento agendado para o proximo turno.");
        }
    }
    @Override
    /**
     * Executa a ação de uma unidade, permitindo que o jogador escolha uma unidade 
     * para mover.
     *
     * Este método solicita ao jogador que escolha uma unidade a partir de uma 
     * lista de unidades disponíveis. Se a unidade estiver ocupada, uma mensagem 
     * de erro é exibida. O jogador também deve fornecer as coordenadas de 
     * destino para a unidade.
     *
     * @param tipoUnidade o tipo da unidade que está realizando a ação
     * @param listaUnidades a lista de unidades disponíveis para seleção
     */
    public void acaoUnidade(String tipoUnidade, List<IUnidade> listaUnidades) {
        // Solicita ao jogador que escolha a unidade
        GestorMensagens.mostrarMensagem("Escolha o numero da unidade para mover:");
        int escolha = s.nextInt();
        s.nextLine(); // Limpa o buffer
        // Valida a escolha do jogador
        if (escolha < 1 || escolha > listaUnidades.size()) {
            GestorMensagens.mostrarMensagem("Escolha invalida!");
            return;
        }
        IUnidade unidadeSelecionada = listaUnidades.get(escolha - 1);

        if (unidadeSelecionada.isOcupada()) {
            GestorMensagens.mostrarMensagem("A unidade escolhida esta ocupada.");
            return;
        }
        // Exibe o mapa e a posição atual da unidade selecionada
        mapa.mostrarMapaZoom(unidadeSelecionada.getPosX(), unidadeSelecionada.getPosY(), 6);
        GestorMensagens.mostrarMensagem("Posicao atual: (" + unidadeSelecionada.getPosX() + 
                ", " + unidadeSelecionada.getPosY() + ")");
        GestorMensagens.mostrarMensagem("Digite a posicao X e Y de destino:");
        int x = s.nextInt();
        int y = s.nextInt();
        // Move a unidade para a posição especificada
        moverUnidade(unidadeSelecionada, x, y);
    }

    /**
     * Processa os movimentos planejados.
     *
     * Este método executa todos os movimentos agendados, movendo cada unidade 
     * para sua posição de destino e liberando-a após o movimento. A lista de 
     * movimentos planejados é limpa após o processamento.
     */
    public void processarMovimentos() {
        for (AgendaAcao movimento : movimentosPlaneados) {
            IUnidade unidade = movimento.getUnidade();
            int destinoX = movimento.getDestinoX();
            int destinoY = movimento.getDestinoY();

            unidade.mover(mapa, destinoX, destinoY);
            unidade.setOcupada(false);
        }
        movimentosPlaneados.clear(); // Limpa os movimentos após processá-los
    }

    /**
     * Verifica se uma unidade pode ser movida para uma posição de destino.
     *
     * Este método valida se a unidade pode se mover para a posição especificada, 
     * verificando se a posição é válida, se o terreno está livre e se o custo 
     * do movimento está dentro do limite permitido.
     *
     * @param unidade a unidade a ser movida
     * @param destinoX a coordenada X de destino
     * @param destinoY a coordenada Y de destino
     * @return true se a unidade pode ser movida, false caso contrário
     */
    public boolean podeMoverUnidade(IUnidade unidade, int destinoX, int destinoY) {        
        if (!mapa.posicaoValida(destinoX, destinoY)) {
            GestorMensagens.mostrarMensagem("Movimento impossivel: destino inacessivel.");
            return false; // Fora do mapa
        }

        if (!terrenoLivre(destinoX, destinoY)) {
            GestorMensagens.mostrarMensagem("Movimento impossivel: posicao ja ocupada");
            return false; // Posição ocupada
        }
        boolean possibilidade = calcularCustoMovimento(unidade.getPosX(), unidade.getPosY(), 
                destinoX, destinoY) <= MAX_MOVIMENTO;
        if (!possibilidade) {
            GestorMensagens.mostrarMensagem("Movimento impossivel: custo demasiado elevado!");
        }
        return possibilidade;
    }

    /**
     * Verifica se o terreno de destino está livre para movimento.
     *
     * Este método verifica se o terreno na posição de destino está livre, ou seja, 
     * se não há alvos ou obstáculos que impeçam o movimento.
     *
     * @param destinoX a coordenada X do terreno de destino
     * @param destinoY a coordenada Y do terreno de destino
     * @return true se o terreno está livre, false caso contrário
     */
    private boolean terrenoLivre(int destinoX, int destinoY) {
        ITerreno terreno = mapa.getTerreno(destinoX, destinoY);

        if (terreno.temAlvo()) {
            return false; // O terreno tem um alvo, portanto, está ocupado.
        }
        String overlaySimbolo = terreno.getOverlaySimbolo();
        if (!overlaySimbolo.equals("") && 
                !overlaySimbolo.equals(ITerreno.SIMBOLO_ESTRADA)) {
            return false; // O overlay não é vazio e não é uma estrada.
        }
        for (AgendaAcao movimento : movimentosPlaneados) {
            if ((movimento.getDestinoX() == destinoX) && (movimento.getDestinoY() == destinoY))
                return false;
        }
        return true;
    }
    /**
    * Obtém a lista de movimentos planeados.
    *
    * Este método retorna a lista de ações de movimento que foram agendadas. 
    * Essa lista pode ser utilizada para verificar quais movimentos estão 
    * programados para serem executados.
    *
    * @return a lista de movimentos planejados
    */
    public static List<AgendaAcao> getMovimentosPlaneados() {
        return movimentosPlaneados;
    }
    
    /**
     * Calcula o custo de movimento entre duas posições no mapa usando BFS.
     * 
     * @param origemX Coordenada X de origem.
     * @param origemY Coordenada Y de origem.
     * @param destinoX Coordenada X de destino.
     * @param destinoY Coordenada Y de destino.
     * @return O custo de movimento ou Integer.MAX_VALUE se o destino for inacessível.
     */
    private int calcularCustoMovimento(int origemX, int origemY, int destinoX, int destinoY) {
        int[][] direcoes = {{0, 1}, {1, 0}, {0, -1}, {-1, 0}};
        int[][] custoMinimo = new int[mapa.getAltura()][mapa.getLargura()];
        for (int[] linha : custoMinimo) {
            Arrays.fill(linha, Integer.MAX_VALUE);
        }
        PriorityQueue<int[]> fila = new PriorityQueue<>(Comparator.comparingInt(a -> a[2])); // {x, y, custo}
        fila.add(new int[]{origemX, origemY, 0});
        custoMinimo[origemY][origemX] = 0;

        while (!fila.isEmpty()) {
            int[] atual = fila.poll();
            int x = atual[0], y = atual[1], custoAtual = atual[2];

            if (x == destinoX && y == destinoY) {
                return custoAtual; // Encontrou o destino com o menor custo
            }
            if (custoAtual > custoMinimo[y][x]) {
                continue; // Ignorar caminhos que não são os de menor custo
            }
            for (int[] dir : direcoes) {
                int novoX = (x + dir[0] + mapa.getLargura()) % mapa.getLargura(); // Circular horizontalmente
                int novoY = y + dir[1];

                if (mapa.posicaoValida(novoX, novoY)) {
                    int custoNovo = custoAtual + mapa.getTerreno(novoX, novoY).getCustoMovimento();
                    if (custoNovo < custoMinimo[novoY][novoX]) {
                        custoMinimo[novoY][novoX] = custoNovo;
                        fila.add(new int[]{novoX, novoY, custoNovo});
                    }
                }
            }
        }
        return Integer.MAX_VALUE; // Destino inacessível
    }
}
