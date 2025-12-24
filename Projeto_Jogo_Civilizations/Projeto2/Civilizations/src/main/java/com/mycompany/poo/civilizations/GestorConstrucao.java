/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;
import java.io.*;
import java.util.Map;

/**
 *
 * @author Diogo
 */
public class GestorConstrucao {
    private IMapa mapa;
    private Scanner s;
    private static List<AgendaAcao> construcoesPlaneadas;

    public GestorConstrucao(IMapa mapa) {
        this.mapa = mapa;
        s = new Scanner(System.in); 
        construcoesPlaneadas = new ArrayList<>();
    }    
    /**
    * Grava as construções planeadas em um arquivo.
    *
    * Este método escreve as ações de construção em um arquivo especificado, 
    * formatando os dados de cada ação como uma string. Cada linha do arquivo 
    * representa uma ação de construção.
    *
    * @param caminhoArquivo o caminho do arquivo onde as construções serão gravadas
    * @throws IOException se ocorrer um erro de entrada/saída durante a gravação
    */
   public static void gravarConstrucoes(String caminhoArquivo) throws IOException {
       try (BufferedWriter writer = new BufferedWriter(new FileWriter(caminhoArquivo))) {
           for (AgendaAcao acao : construcoesPlaneadas) {
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
           System.err.println("Erro ao gravar construcoes: " + e.getMessage());
       }
   }

   /**
    * Carrega as construções planeadas de um arquivo.
    *
    * Este método lê as ações de construção de um arquivo e as adiciona à lista 
    * de construções planejadas. Cada linha do arquivo deve conter os dados de 
    * uma ação de construção formatados corretamente.
    *
    * @param reader o BufferedReader usado para ler os dados do arquivo
    * @param unidades um mapa que relaciona tipos de unidades a suas instâncias
    * @throws IOException se ocorrer um erro de entrada/saída durante a leitura
    */
   public static void carregarConstrucoes(BufferedReader reader, Map<String, List<IUnidade>> unidades) throws IOException {
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
                       construcoesPlaneadas.add(new AgendaAcao(unidade, destinoX, destinoY));                            
                   }
               }
           }                   
       }       
   }

   /**
    * Realiza a construção de uma unidade selecionada.
    *
    * Este método solicita ao jogador que selecione uma unidade do tipo especificado 
    * para realizar a construção. Se a seleção for válida, a construção é agendada.
    *
    * @param tipoUnidade o tipo da unidade que será utilizada para a construção
    * @param listaUnidades a lista de unidades disponíveis para seleção
    */
   public void realizarConstrucao(String tipoUnidade, List<IUnidade> listaUnidades) {
       GestorMensagens.mostrarMensagem("Selecione o indice da unidade " + tipoUnidade + 
               " para construir:");
       int indice = s.nextInt() - 1; // Convertendo para índice baseado em 0

       if (indice < 0 || indice >= listaUnidades.size()) {
           GestorMensagens.mostrarMensagem("Indice invalido.");
           return;
       }
       IUnidade construtorSelecionado = listaUnidades.get(indice);

       mapa.mostrarMapaZoom(construtorSelecionado.getPosX(), construtorSelecionado.getPosY(), 2);
       GestorMensagens.mostrarMensagem("Posicao atual: (" + construtorSelecionado.getPosX() + 
           ", " + construtorSelecionado.getPosY() + ")");

       construir(construtorSelecionado);
   }

   /**
    * Processa as construções planeadas.
    *
    * Este método executa as construções agendadas, chamando o método de construção 
    * apropriado para cada unidade e limpando a lista de construções planeadas após 
    * a execução.
    */
    public void processarConstrucoes() {
        for (AgendaAcao construcao : construcoesPlaneadas) {
            IUnidade unidade = construcao.getUnidade();
            int destinoX = construcao.getDestinoX();
            int destinoY = construcao.getDestinoY();
            
            ((UnidadeConstrutor)unidade).construirEstrada(mapa, mapa.getTerreno(destinoX, destinoY));
            unidade.setOcupada(false);
        }
        construcoesPlaneadas.clear(); // Limpa os movimentos após processá-los
    }
    /**
    * Seleciona um terreno a partir de uma lista de terrenos disponíveis.
    *
    * Este método exibe os terrenos próximos e solicita ao jogador que escolha 
    * um deles. Se a escolha for inválida, uma mensagem de erro é exibida.
    *
    * @param terrenos a lista de terrenos disponíveis para seleção
    * @return o terreno selecionado ou null se a escolha for inválida
    */
   private ITerreno selecionarTerreno(List<ITerreno> terrenos) {
       GestorMensagens.mostrarMensagem("Terrenos proximos:");
       for (int i = 0; i < terrenos.size(); i++) {
           ITerreno terreno = terrenos.get(i);
           GestorMensagens.mostrarMensagem((i + 1) + ". " + terreno.toString() + " (" + 
                   terreno.getX() + ", " + terreno.getY() + ")");
       }
       GestorMensagens.mostrarMensagem("Escolha o terreno:");
       int escolha = s.nextInt();
       s.nextLine(); // Limpa o buffer

       if (escolha < 1 || escolha > terrenos.size()) {
           GestorMensagens.mostrarMensagem("Escolha invalida!");
           return null;
       }
       return terrenos.get(escolha - 1);
   }

   /**
    * Inicia a construção de uma estrada utilizando a unidade especificada.
    *
    * Este método verifica se a unidade selecionada é uma unidade construtora. 
    * Se for, ele obtém os terrenos próximos e remove os terrenos do tipo 
    * TerrenoCidade da lista. Em seguida, solicita ao jogador que selecione 
    * um terreno e agenda a construção para o próximo turno.
    *
    * @param unidade a unidade que realizará a construção
    */
   private void construir(IUnidade unidade) {
       if (!(unidade instanceof UnidadeConstrutor)) {
           GestorMensagens.mostrarMensagem("Apenas unidades construtoras podem construir estradas!");
           return;
       }
       List<ITerreno> terrenos = mapa.getTerrenosProximos(unidade.getPosX(), unidade.getPosY(), 1);
       // Remove os terrenos do tipo TerrenoCidade
       terrenos.removeIf(terreno -> terreno instanceof TerrenoCidade);

       ITerreno terrenoSelecionado = selecionarTerreno(terrenos);
       construcoesPlaneadas.add(new AgendaAcao(unidade, terrenoSelecionado.getX(), 
               terrenoSelecionado.getY()));
       GestorMensagens.mostrarMensagem("Construcao agendada para o proximo turno.");
   }

   /**
    * Obtém a lista de construções planejadas.
    *
    * Este método retorna a lista de ações de construção que foram agendadas.
    *
    * @return a lista de construções planejadas
    */
   public static List<AgendaAcao> getConstrucoesPlaneadas() {
       return construcoesPlaneadas;
   }
}
