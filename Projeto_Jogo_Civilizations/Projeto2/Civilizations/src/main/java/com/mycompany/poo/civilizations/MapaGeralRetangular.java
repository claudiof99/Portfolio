/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.util.*;

/**
 *
 * @author Diogo
 */
public class MapaGeralRetangular implements IMapa, IEstadoJogo{
    private ITerreno[][] mapa;    
    private ArrayList<Cidade> cidades;
    private int largura;
    private int altura;

    // Construtor que já inicializa o mapa com terrenos padrões
    public MapaGeralRetangular(int largura, int altura) {
        this.largura = largura;
        this.altura = altura;
        mapa = new ITerreno[largura][altura];
        iniTerrenosMapa(mapa, largura, altura);
        cidades = new ArrayList<Cidade>(); // Inicializa a lista de cidades
    }
    /**
    * Método para gravar o estado do mapa em um arquivo.
    *
    * Este método escreve as dimensões do mapa e os tipos de terrenos em um 
    * arquivo, incluindo os overlays de cada terreno.
    *
    * @param writer o BufferedWriter usado para gravar os dados
    * @throws IOException se ocorrer um erro de entrada/saída durante a gravação
    */
   @Override
   public void gravaDados(BufferedWriter writer) throws IOException {
       // Escreve as dimensões do mapa
       writer.write(largura + ";" + altura);
       writer.newLine();
       // Escreve os tipos de terrenos
       for (int y = 0; y < altura; y++) {
           for (int x = 0; x < largura; x++) {
               writer.write(mapa[x][y].getClass().getSimpleName()); // Nome da classe
               // Adiciona os overlays
               Stack<String> overlays = mapa[x][y].getOverlays();
               if (!overlays.isEmpty()) {
                   writer.write("|" + String.join(",", overlays) + "|");
               }               
               if (x < largura - 1) {
                   writer.write(";"); // Separador
               }
           }
           writer.newLine();
       }
   }

   /**
    * Método para carregar o estado do mapa a partir de um arquivo.
    *
    * Este método lê as dimensões do mapa e os tipos de terrenos de um arquivo, 
    * criando um novo mapa com base nas informações lidas.
    *
    * @param reader o BufferedReader usado para ler os dados do arquivo
    * @param mapa o mapa a ser carregado
    * @param jogador indica se o jogador está carregando o estado
    * @return o novo mapa carregado
    * @throws IOException se ocorrer um erro de entrada/saída durante a leitura
    */
   @Override
   public IMapa carregarEstado(BufferedReader reader, IMapa mapa, boolean jogador) throws IOException {       
       // Lê as dimensões do mapa na primeira linha
       String dimensoesLinha = reader.readLine();
       String[] dimensoes = dimensoesLinha.split(";");
       int largura = Integer.parseInt(dimensoes[0]);
       int altura = Integer.parseInt(dimensoes[1]);
       // Cria um novo mapa
       IMapa novoMapa = new MapaGeralRetangular(largura, altura);
       // Lê os terrenos linha por linha
       for (int y = 0; y < altura; y++) {
           String linha = reader.readLine(); // Lê a linha correspondente ao y
           String[] terrenos = linha.split(";");

           for (int x = 0; x < largura; x++) {
               String entrada = terrenos[x];
               String tipoTerreno;
               Stack<String> overlays = new Stack<>();
               // Verifica se há overlays na entrada
               if (entrada.contains("|")) {
                   String[] partes = entrada.split("\\|");
                   tipoTerreno = partes[0].trim(); // Tipo do terreno antes do "|"
                   if (partes.length > 1 && !partes[1].isEmpty()) {
                       // Adiciona os overlays (se existirem)
                       overlays.addAll(Arrays.asList(partes[1].split(",")));
                   }
               } else {
                   tipoTerreno = entrada.trim(); // Apenas o tipo de terreno
               }
               ITerreno terreno;
               // Instancia o tipo de terreno com base no nome
               switch (tipoTerreno) {
                   case "TerrenoAgua":
                       terreno = new TerrenoAgua(x, y);
                       break;
                   case "TerrenoFloresta":
                       terreno = new TerrenoFloresta(x, y);
                       break;
                   case "TerrenoMontanha":
                       terreno = new TerrenoMontanha(x, y);
                       break;
                   case "TerrenoPlanicie":
                       terreno = new TerrenoPlanicie(x, y);
                       break;
                   case "TerrenoCidade":
                       terreno = new TerrenoCidade(x, y);
                       break;
                   default:
                       throw new IllegalArgumentException("Tipo de terreno desconhecido: " + tipoTerreno);
               }
               // Associa os overlays ao terreno
               terreno.setOverlays(overlays);
               // Define o terreno no mapa
               novoMapa.setTerreno(x, y, terreno);
           }
       }
       return novoMapa;       
   }

   /**
    * Inicializa os terrenos do mapa com terrenos aleatórios.
    *
    * Este método preenche o mapa com terrenos aleatórios, escolhendo entre 
    * diferentes tipos de terrenos com base em uma distribuição uniforme.
    *
    * @param mapa a matriz de terrenos a ser inicializada
    * @param largura a largura do mapa
    * @param altura a altura do mapa
    */
    @Override
    public void iniTerrenosMapa(ITerreno[][] mapa, int largura, int altura) {
        // Criação de uma instância da classe Random
        Random rand = new Random();
        // Inicialização das células do mapa com terrenos aleatórios
        for (int x = 0; x < largura; x++) {
            for (int y = 0; y < altura; y++) {
                // Gerar um número aleatório entre 0 e 3 para escolher o tipo de terreno
                int randomNum = rand.nextInt(4); // Gera um número aleatório entre 0 e 3
                // Switch para determinar o tipo de terreno
                switch (randomNum) {
                    case 0:
                        mapa[x][y] = new TerrenoAgua(x, y);   // 25% prob. de ser água
                        break;
                    case 1:
                        mapa[x][y] = new TerrenoFloresta(x, y); // 25% prob. de ser floresta
                        break;
                    case 2:
                        mapa[x][y] = new TerrenoMontanha(x, y); // 25% prob. de ser montanha
                        break;
                    case 3:
                        mapa[x][y] = new TerrenoPlanicie(x, y);  // 25% prob. de ser planície
                        break;
                    default:
                        // Caso algum valor inesperado apareça, o mapa seria configurado como planície
                        mapa[x][y] = new TerrenoPlanicie(x, y);
                        break;
                }
            }
        }
    }
    /**
    * Obtém o terreno na posição especificada.
    *
    * Este método ajusta a coordenada X para um movimento circular e verifica 
    * se a coordenada Y está dentro dos limites do mapa. Se a posição for 
    * válida, retorna o terreno correspondente.
    *
    * @param x a coordenada X do terreno
    * @param y a coordenada Y do terreno
    * @return o terreno na posição ajustada ou null se a posição for inválida
    */
   @Override
   public ITerreno getTerreno(int x, int y) {
       // Ajusta o x para o movimento circular, usando o operador de módulo
       int posX = (x + largura) % largura;
       // Verifica se y está dentro dos limites verticais do mapa
       if (y >= 0 && y < altura) {
           return mapa[posX][y]; // Retorna o terreno na posição ajustada
       }
       return null; // Retorna null se y estiver fora dos limites
   }

   /**
    * Define um terreno na posição especificada.
    *
    * Este método verifica se as coordenadas estão dentro dos limites do mapa 
    * e, se estiverem, define o novo terreno na posição especificada. Caso 
    * contrário, uma mensagem de erro é exibida.
    *
    * @param x a coordenada X do terreno
    * @param y a coordenada Y do terreno
    * @param terreno o terreno a ser definido na posição especificada
    */
   @Override
   public void setTerreno(int x, int y, ITerreno terreno) {
       // Verifica se as coordenadas estão dentro dos limites do mapa
       if (x >= 0 && x < largura && y >= 0 && y < altura) {
           mapa[x][y] = terreno; // Define o novo terreno na posição especificada
       } else {
           GestorMensagens.mostrarMensagem("\nPosicao fora dos limites do mapa.");
       }
   }

   /**
    * Obtém os terrenos próximos a uma posição especificada dentro de uma 
    * distância máxima.
    *
    * Este método retorna uma lista de terrenos que estão dentro da distância 
    * máxima especificada em relação à posição dada.
    *
    * @param posX a coordenada X da posição central
    * @param posY a coordenada Y da posição central
    * @param distanciaMaxima a distância máxima para buscar terrenos
    * @return uma lista de terrenos próximos
    */
   @Override
   public ArrayList<ITerreno> getTerrenosProximos(int posX, int posY, int distanciaMaxima) {
       ArrayList<ITerreno> terrenosProximos = new ArrayList<>();

       for (int x = Math.max(0, posX - distanciaMaxima); x <= Math.min(largura - 1, posX + distanciaMaxima); x++) {
           for (int y = Math.max(0, posY - distanciaMaxima); y <= Math.min(altura - 1, posY + distanciaMaxima); y++) {           
               ITerreno terreno = mapa[x][y];
               if (terreno != null) {
                   terrenosProximos.add(terreno);
               }
           }
       }
       return terrenosProximos;
   }

   /**
    * Exibe o mapa no console.
    *
    * Este método percorre todas as posições do mapa e imprime o símbolo 
    * correspondente a cada terreno.
    */
   @Override
   public void mostrarMapa() {
       for (int y = 0; y < altura; y++) { // Percorre as linhas do mapa
           for (int x = 0; x < largura; x++) { // Percorre as colunas do mapa
               // Imprime o símbolo do terreno na posição (x, y)
               System.out.print(mapa[x][y].getSimbolo() + " "); 
           }
           // Vai para a linha seguinte após imprimir uma linha completa do mapa
           System.out.println(); 
       }
   }

   /**
    * Adiciona uma cidade ao mapa na posição especificada.
    *
    * Este método verifica se a posição é válida e se respeita a distância 
    * mínima entre cidades. Se a posição for válida, uma nova cidade é criada 
    * e adicionada ao mapa e à civilização correspondente.
    *
    * @param civilizacao a civilização à qual a cidade pertence
    * @param nome o nome da nova cidade
    * @param posX a coordenada X da nova cidade
    * @param posY a coordenada Y da nova cidade
    * @param inimigo indica se a cidade é inimiga
    */
    @Override
    public void adicionarCidade(Civilizacao civilizacao, String nome, int posX, int posY, 
            boolean inimigo) {
        // Verifica se a posição é válida e se respeita a distância mínima entre cidades
        if (posicaoValidaParaCidade(posX, posY)) {
            Cidade cidade = new Cidade(nome, posX, posY, this, civilizacao);
            ITerreno terreno = new TerrenoCidade(posX, posY);
            if (inimigo) {terreno.adicionarAlvo(cidade);}
            cidades.add(cidade);
            civilizacao.adicionarCidade(cidade);
            setTerreno(posX, posY, terreno); // Adiciona terreno Cidade no mapa
            ((ITerrenoCidade)terreno).setCidade(cidade);
            GestorMensagens.mostrarMensagem("\nCidade adicionada com sucesso.");
        }         
    }
    /**
     * Obtém a cidade na posição especificada.
     *
     * Este método verifica se existe uma cidade na coordenada (x, y) da 
     * civilização fornecida. Se uma cidade for encontrada, ela é retornada; 
     * caso contrário, retorna null.
     *
     * @param civilizacao a civilização à qual a cidade pertence
     * @param x a coordenada X da cidade
     * @param y a coordenada Y da cidade
     * @return a cidade na posição especificada ou null se nenhuma cidade for encontrada
     */
    @Override
    public Cidade getCidadePorCoordenada(Civilizacao civilizacao, int x, int y) {
        for (Cidade cidade : civilizacao.getCidades()) {
            if (cidade.getPosX() == x && cidade.getPosY() == y) {
                return cidade;
            }
        }
        return null; // Retorna null se nenhuma cidade for encontrada nessa posição
    }

    /**
     * Verifica se existem cidades na civilização.
     *
     * Este método verifica se a lista de cidades está vazia. Se não houver 
     * cidades disponíveis, uma mensagem é exibida e o método retorna false; 
     * caso contrário, retorna true.
     *
     * @return true se houver cidades disponíveis, false caso contrário
     */
    @Override
    public boolean existemCidades() {
        if (cidades.isEmpty()) {
            GestorMensagens.mostrarMensagem("\nNenhuma cidade disponível.");
            return false;
        } else {
            return true;
        }
    }

    /**
     * Exibe uma visão ampliada do mapa em torno de uma posição central.
     *
     * Este método itera sobre as linhas e colunas dentro do raio especificado 
     * em relação à posição central e imprime os símbolos dos terrenos. Se a 
     * posição estiver fora dos limites do mapa, um ponto (.) é exibido.
     *
     * @param centroX a coordenada X do centro da visão
     * @param centroY a coordenada Y do centro da visão
     * @param raio a distância máxima em torno do centro a ser exibida
     */
    @Override
    public void mostrarMapaZoom(int centroX, int centroY, int raio) {
        // Itera sobre as linhas e colunas dentro do raio especificado
        for (int y = centroY - raio; y <= centroY + raio; y++) {
            for (int x = centroX - raio; x <= centroX + raio; x++) {
                // Ajusta x com modularidade para suportar mapas circulares horizontalmente
                int posX = (x + largura) % largura;
                // Verifica se y está dentro dos limites do mapa vertical
                if (y >= 0 && y < altura) {
                    System.out.print(mapa[posX][y].getSimbolo() + " "); // Mostra o símbolo
                } else {
                    System.out.print(" . "); // Fora do limite vertical, exibe "."
                }
            }
            System.out.println();
        }
    }

    /**
     * Obtém a altura do mapa.
     *
     * @return a altura do mapa
     */
    @Override
    public int getAltura() {
        return altura;
    }

    /**
     * Obtém a largura do mapa.
     *
     * @return a largura do mapa
     */
    @Override
    public int getLargura() {
        return largura;
    }

    /**
     * Obtém a lista de cidades no mapa.
     *
     * @return uma lista de cidades
     */
    @Override
    public ArrayList<Cidade> getCidades() {
        return cidades;
    }

    /**
     * Verifica se a posição especificada é válida.
     *
     * Este método verifica se a posição (x, y) está dentro dos limites do mapa 
     * e se o terreno na posição é acessível.
     *
     * @param x a coordenada X da posição a ser verificada
     * @param y a coordenada Y da posição a ser verificada
     * @return true se a posição for válida, false caso contrário
     */
    @Override
    public boolean posicaoValida(int x, int y) {
        ITerreno terreno = getTerreno(x, y);
        if (terreno == null) {
            return false; // A posição não é válida se o terreno não existe
        }
        return terreno.eAcessivel(); // Verifica a validade com base no custo
    }

    /**
     * Verifica se a posição especificada é válida para a construção de uma cidade.
     *
     * Este método verifica se a posição (posX, posY) é válida e se respeita a 
     * distância mínima entre cidades. Se a posição for válida, retorna true; 
     * caso contrário, retorna false.
     *
     * @param posX a coordenada X da posição a ser verificada
     * @param posY a coordenada Y da posição a ser verificada
     * @return true se a posição for válida para a construção de uma cidade, 
     *         false caso contrário
     */
    @Override
    public boolean posicaoValidaParaCidade(int posX, int posY) {
        if(!posicaoValida(posX, posY)) { return false;}
        
        for (Cidade cidade : cidades) {
            int distanciaX = Math.abs(cidade.getPosX() - posX);
            int distanciaY = Math.abs(cidade.getPosY() - posY);
            if (distanciaX <= Cidade.getDistanciaMinima() && distanciaY <= Cidade.getDistanciaMinima()) {
                GestorMensagens.mostrarMensagem("\nJa existe uma cidade nas proximidades(menos de " 
                        + (Cidade.getDistanciaMinima() + 1) + " celulas).");
                return false;
            }
        }
        return true;
    }
}
