/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.io.*;
import java.util.ArrayList;
/**
 *
 * @author Diogo
 */
public class Civilizacao implements IEstadoJogo{
    private String nome;
    private int tesouro; // Tesouro da civilização
    private double multiplicadorProducao; // Multiplicador aplicado a produções
    private ArrayList<Cidade> cidades; // Lista de cidades associadas   
    // Construtor
    public Civilizacao(String nome) {
        this.nome = nome;
        this.tesouro = 0; // Inicialmente vazio
        this.multiplicadorProducao = 1.0; // Sem multiplicador por padrão
        this.cidades = new ArrayList<>();
    }   
    // Remover uma cidade da civilização
    /**
     * Remove uma cidade da civilização.
     *
     * Este método remove a cidade especificada da lista de cidades da civilização.
     *
     * @param cidade a cidade a ser removida
     */
    private void removerCidade(Cidade cidade) {
        cidades.remove(cidade);
    }

    /**
     * Método para gravar o estado da civilização em um arquivo.
     *
     * Este método grava as informações da civilização, incluindo seu nome, 
     * tesouro e multiplicador de produção, além de gravar os dados de cada 
     * cidade pertencente a ela.
     *
     * @param writer o BufferedWriter usado para gravar os dados
     * @throws IOException se ocorrer um erro de entrada/saída durante a gravação
     */
    @Override
    public void gravaDados(BufferedWriter writer) throws IOException {
        writer.write("Civilizacao");
        writer.newLine();
        writer.write("Civilizacao;" + nome + ";" + tesouro + ";" + multiplicadorProducao);
        writer.newLine();
        for (Cidade cidade : cidades) {
            writer.write("Cidade;" + cidade.getNome() + ";" + cidade.getPosX() + ";" + cidade.getPosY());
            writer.newLine();
            cidade.gravaDados(writer);
        }
    }

    /**
     * Método para carregar o estado da civilização a partir de um arquivo.
     *
     * Este método lê os dados da civilização de um BufferedReader e 
     * reconstrói a civilização e suas cidades a partir dessas informações.
     *
     * @param reader o BufferedReader usado para ler os dados
     * @param mapa o mapa onde as cidades estão localizadas
     * @param jogador indica se a civilização é do jogador
     * @return a civilização carregada
     * @throws IOException se ocorrer um erro de entrada/saída durante a leitura
     */
    @Override
    public Civilizacao carregarEstado(BufferedReader reader, IMapa mapa, boolean jogador) throws IOException {
        String linha;
        do {
            if ((linha = reader.readLine()).equals("Civilizacao")) {
                linha = reader.readLine();
            }
            String[] partes = linha.split(";");
            Civilizacao civilizacao = new Civilizacao(partes[1]);
            civilizacao.setTesouroOuro(Integer.parseInt(partes[2]));
            civilizacao.setMultiplicadorProducao(Double.parseDouble(partes[3]));
            linha = reader.readLine();
            // Adicionar as cidades à civilização
            do {
                if (linha.equals("Cidade")) {
                    linha = reader.readLine();
                }
                String[] partesCidade = linha.split(";");
                if (partesCidade[0].equals("Cidade")) {
                    String nomeCidade = partesCidade[1];
                    int posX = Integer.parseInt(partesCidade[2]);
                    int posY = Integer.parseInt(partesCidade[3]);
                    // Criar a cidade e adicionar à civilização
                    Cidade cidade = new Cidade(nomeCidade, posX, posY, mapa, civilizacao);
                    civilizacao.adicionarCidade(cidade);
                    if (jogador) {
                        ((ITerrenoCidade) mapa.getTerreno(posX, posY)).setCidadeJogador();
                    }
                    // Carregar estado da cidade sem perder a linha da próxima cidade
                    while ((linha = reader.readLine()) != null && !linha.startsWith("Cidade") 
                            && !linha.startsWith("Civilizacao")) {
                        cidade.processarLinhaEstado(linha, reader); // Novo método para processar dados da cidade
                    }
                    ((ITerrenoCidade) mapa.getTerreno(posX, posY)).setCidade(cidade);
                    mapa.getCidades().add(cidade);
                    // A linha lida pode ser a próxima cidade, portanto, continua no loop
                    if (linha != null && linha.startsWith("Cidade")) {
                        continue;
                    } else {
                        break;
                    }
                }
            } while (linha != null && !linha.startsWith("Civilizacao"));
            return civilizacao;
        } while (linha.startsWith("Civilizacao"));
    }

    /**
     * Adiciona uma quantidade de ouro ao tesouro da civilização.
     *
     * Este método aumenta o tesouro da civilização pela quantidade especificada.
     *
     * @param quantidade a quantidade de ouro a ser adicionada
     */
    public void adicionarOuro(int quantidade) {
        this.tesouro += quantidade;
    }

    /**
     * Consome uma quantidade de ouro do tesouro da civilização.
     *
     * Este método reduz o tesouro da civilização pela quantidade especificada, 
     * se houver ouro suficiente. Caso contrário, uma mensagem de erro é exibida.
     *
     * @param quantidade a quantidade de ouro a ser consumida
     */
    public void consumirOuro(int quantidade) {
        if (tesouro >= quantidade) {
            tesouro -= quantidade;
        } else {
            GestorMensagens.mostrarMensagem("Ouro insuficiente no tesouro da civilizacao!");
        }
    }
    
    /**
    * Verifica se existem cidades na civilização.
    *
    * Este método retorna verdadeiro se a lista de cidades não estiver vazia, 
    * indicando que a civilização possui pelo menos uma cidade.
    *
    * @return verdadeiro se existem cidades, falso caso contrário
    */
   public boolean existemCidades() {
       return !cidades.isEmpty();
   }

   /**
    * Adiciona uma cidade à civilização.
    *
    * Este método adiciona a cidade especificada à lista de cidades da civilização.
    *
    * @param cidade a cidade a ser adicionada
    */
   public void adicionarCidade(Cidade cidade) {
       cidades.add(cidade);
   }

   /**
    * Associa as cidades da civilização ao mapa.
    *
    * Este método atualiza o mapa para refletir a posição das cidades da 
    * civilização, definindo-as como cidades do jogador no terreno correspondente.
    *
    * @param mapa o mapa onde as cidades estão localizadas
    */
   public void associarCidades(IMapa mapa) {
       for (Cidade cidade : cidades) {
           int x = cidade.getPosX();
           int y = cidade.getPosY();
           ((ITerrenoCidade) mapa.getTerreno(x, y)).setCidadeJogador();
       }
   }

   /**
    * Conta o número de cidades inimigas em relação a uma civilização específica.
    *
    * Este método percorre uma lista de todas as cidades e conta quantas 
    * delas não pertencem à civilização especificada.
    *
    * @param civilizacao a civilização para a qual se deseja contar cidades inimigas
    * @param todasAsCidades a lista de todas as cidades disponíveis
    * @return o número de cidades inimigas
    */
   public int contarCidadesInimigas(Civilizacao civilizacao, ArrayList<Cidade> todasAsCidades) {
       int cidadesInimigas = 0;
       for (Cidade cidade : todasAsCidades) {
           if (!civilizacao.getCidades().contains(cidade)) {
               cidadesInimigas++;
           }
       }
       return cidadesInimigas;
   }

   /**
    * Obtém o multiplicador de produção da civilização.
    *
    * @return o multiplicador de produção
    */
   public double getMultiplicadorProducao() {
       return multiplicadorProducao;
   }

   /**
    * Define o multiplicador de produção da civilização.
    *
    * @param multiplicadorProducao o novo multiplicador de produção a ser definido
    */
   public void setMultiplicadorProducao(double multiplicadorProducao) {
       this.multiplicadorProducao = multiplicadorProducao;
   }

   /**
    * Obtém o nome da civilização.
    *
    * @return o nome da civilização
    */
   public String getNome() {
       return nome;
   }

   /**
    * Obtém a quantidade de ouro no tesouro da civilização.
    *
    * @return a quantidade de ouro no tesouro
    */
   public int getTesouroOuro() {
       return tesouro;
   }

   /**
    * Define a quantidade de ouro no tesouro da civilização.
    *
    * @param tesouro a nova quantidade de ouro a ser definida
    */
   public void setTesouroOuro(int tesouro) {
       this.tesouro = tesouro;
   }

   /**
    * Obtém a lista de cidades da civilização.
    *
    * @return a lista de cidades
    */
   public ArrayList<Cidade> getCidades() {
       return cidades;
   }

   /**
    * Obtém uma string com os nomes das cidades da civilização.
    *
    * @return uma string contendo os nomes das cidades, separados por " | "
    */
   public String getNomeCidades() {
       String texto = "";
       for (Cidade c : cidades) {
           texto += c.getNome() + " | ";
       }
       return texto;
   }

   /**
    * Obtém a população total de todas as cidades da civilização.
    *
    * @return a população total
    */
   public int getTotalPopulacao() {
       int total = 0;
       for (Cidade c : cidades) {
           total += c.getPopulacao().getTotal();
       }
       return total;
   }

   /**
    * Retorna uma representação em string da civilização.
    *
    * Este método fornece uma descrição detalhada da civilização, incluindo 
    * seu nome, cidades, população total e tesouro.
    *
    * @return uma string representando a civilização
    */
   public String toString() {
       String texto = "\nCIVILIZACAO: " + nome;
       texto += "\nCidades: " + getNomeCidades();
       texto += "\nPopulacao total: " + getTotalPopulacao();
       texto += "\nTesouro: " + tesouro;
       return texto;
   }
}
