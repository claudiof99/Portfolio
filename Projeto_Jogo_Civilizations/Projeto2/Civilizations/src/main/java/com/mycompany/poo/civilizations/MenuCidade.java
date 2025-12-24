/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.util.HashMap;
import java.util.Scanner;
import java.util.InputMismatchException;

/**
 *
 * @author Diogo
 */
public class MenuCidade implements IMenu{
    private Cidade cidade;
    private IMapa mapa;
    private HashMap<Integer, ITerreno> terrenosPorId;
    private GestorUnidades unidades;
    private GestorMovimento movimento;
    private GestorAtaque ataque;
    private GestorConstrucao construcao;
    private MenuUnidades menuUnidades;
    private Scanner s;
    
    public MenuCidade(Cidade cidade, IMapa mapa, GestorMovimento movimento, 
            GestorAtaque ataque, GestorConstrucao construcao) {
        this.cidade = cidade;
        this.mapa = mapa;
        this.terrenosPorId = new HashMap<>();
        this.unidades = cidade.getGestorUnidades();
        this.s = new Scanner(System.in);        
        this.movimento = movimento;
        this.ataque = ataque;
        this.construcao = construcao;
        this.menuUnidades = new MenuUnidades(unidades, mapa, cidade, movimento, ataque, 
                                        construcao);
    }
    /**
     * Exibe as opções disponíveis para a cidade atual.
     *
     * Este método mostra o mapa da cidade e apresenta um menu com opções 
     * para o jogador, incluindo ver informações da cidade, gerir a população 
     * e gerir unidades.
     */
    @Override
    public void exibirOpcoes() {
        GestorMensagens.mostrarMensagem("\nMapa da Cidade " + cidade.getCivilizacao().getNome()
            + " " + cidade.getNome());
        mapa.mostrarMapaZoom(cidade.getPosX(), cidade.getPosY(), 3);
        GestorMensagens.mostrarMensagem("\nMenu Cidade ");
        GestorMensagens.mostrarMensagem("1. Ver informacoes da cidade");
        GestorMensagens.mostrarMensagem("2. Gerir populacao");
        GestorMensagens.mostrarMensagem("3. Gerir unidades");
        GestorMensagens.mostrarMensagem("0. Voltar ao Menu Principal");
        GestorMensagens.mostrarMensagem("Escolha uma opcao no Menu Cidade: ");
    }

    /**
     * Processa a escolha do jogador no menu da cidade.
     *
     * Este método exibe as opções disponíveis e executa a ação correspondente 
     * com base na escolha do jogador. O loop continua até que o jogador decida 
     * voltar ao menu principal.
     *
     * @param opcao a opção escolhida pelo jogador
     */
    @Override
    public void processarEscolha(int opcao) {
        boolean continuar = true;
        while (continuar) {
            exibirOpcoes();
            try {
                opcao = s.nextInt();
                switch (opcao) {
                    case 1:
                        GestorMensagens.mostrarMensagem(cidade);
                        break;
                    case 2:
                        GestorMensagens.mostrarMensagem("\nNumero de cidadaos disponiveis: " + cidade.getPopulacao().getPopDisponivel());
                        mostraTerrenos();
                        gerirPopulacao();
                        break;  
                    case 3:
                        unidades.exibirUnidades();
                        menuUnidades.exibirOpcoes();
                        menuUnidades.processarEscolha(opcao);
                        break;    
                    case 0:
                        continuar = false;
                        break;
                    default:
                        GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                        break;
                }
            } catch (InputMismatchException e) {
                GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
                s.nextLine(); // Limpa o buffer de entrada inválida
            }
        }    
    }

    /**
     * Exibe as opções disponíveis para cidades inimigas.
     *
     * Este método mostra o mapa da cidade e apresenta um menu com opções 
     * para o jogador, incluindo ver informações da cidade e voltar ao menu 
     * principal.
     *
     * @param nome o nome da cidade inimiga
     */
    public void exibirOpcoesInimigas(String nome) {
        GestorMensagens.mostrarMensagem("\nMapa da Cidade " + cidade.getCivilizacao().getNome()
            + " " + cidade.getNome());
        mapa.mostrarMapaZoom(cidade.getPosX(), cidade.getPosY(), 3);
        GestorMensagens.mostrarMensagem("\nMenu Cidade ");
        GestorMensagens.mostrarMensagem("1. Ver informacoes da cidade");
        GestorMensagens.mostrarMensagem("0. Voltar ao Menu Principal");
        GestorMensagens.mostrarMensagem("Escolha uma opcao no Menu Cidade: ");
    }

    /**
     * Processa a escolha do jogador no menu de cidades inimigas.
     *
     * Este método exibe as opções disponíveis e executa a ação correspondente 
     * com base na escolha do jogador. O loop continua até que o jogador decida 
     * voltar ao menu principal.
     *
     * @param opcao a opção escolhida pelo jogador
     * @param nome o nome da cidade inimiga
     */
    public void processarEscolhaInimigas(int opcao, String nome) {
        boolean continuar = true;
        while (continuar) {
            exibirOpcoesInimigas(nome);
            try {
                opcao = s.nextInt();
                switch (opcao) {
                    case 1:
                        GestorMensagens.mostrarMensagem(cidade);
                        break; 
                    case 0:
                        GestorMensagens.mostrarMensagem("\nVoltando ao menu principal.");
                        continuar = false;
                        break;
                    default:
                        GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                        break;
                }
            } catch (InputMismatchException e) {
                GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
                s.nextLine(); // Limpa o buffer de entrada inválida
            }
        }    
    }
    /**
    * Exibe os terrenos próximos à cidade.
    *
    * Este método lista os terrenos próximos à cidade, mostrando informações 
    * como o ID do terreno, tipo, coordenadas, recursos disponíveis e o 
    * número de trabalhadores alocados. Os terrenos são associados a um ID 
    * para facilitar a referência.
    */
   private void mostraTerrenos() {
       terrenosPorId = new HashMap<>(); // Inicializa o mapeamento ID -> Terreno

       GestorMensagens.mostrarMensagem("\nTerrenos proximos:");
       GestorMensagens.mostrarFormatado("%-4s | %-10s | %-12s | %-44s | %-13s\n", 
                         "ID", "Tipo", "Coordenadas", "Recursos", "Trabalhadores");
       GestorMensagens.mostrarMensagem("--------------------------------------------------------"
               + "--------------------------------------");
       int id = 1;
       for (ITerreno terreno : mapa.getTerrenosProximos(cidade.getPosX(), cidade.getPosY(), Cidade.getDistMaxAloc())) {
           if (!terreno.toString().equals("Cidade")) {
               terrenosPorId.put(id, terreno); // Associa o terreno ao ID
               GestorMensagens.mostrarFormatado("%-4d | %-10s | (%-2d, %-2d)     | %-15s | %-13d\n",
                       id++, terreno, terreno.getX(), terreno.getY(), 
                       terreno.getRecursosResumo(), terreno.getCidadaosTrabalhando());
           }
       }
   }

   /**
    * Gerencia a população da cidade, permitindo alocação e desalocação de cidadãos.
    *
    * Este método exibe um menu com opções para alocar ou desalocar cidadãos. 
    * Dependendo da escolha do jogador, as ações apropriadas são executadas.
    */
   private void gerirPopulacao() {
       GestorMensagens.mostrarMensagem("\n1. Alocar cidadaos\n2. Desalocar cidadaos\n0. Voltar");
       try {
           int opcao = s.nextInt();
           s.nextLine(); // Limpa o buffer
           switch (opcao) {
               case 1:
                   if (cidade.getPopulacao().getPopDisponivel() > 0) {
                       opcaoAlocar();
                   } else { 
                       GestorMensagens.mostrarMensagem("\nNao existem cidadaos disponíveis para alocar");
                   }                
                   break;
               case 2:
                   if (cidade.getTerrenosTrabalhados().isEmpty()) {
                       GestorMensagens.mostrarMensagem("Nao existem terrenos a serem trabalhados...");
                   } else { 
                       opcaoDesalocar();
                   } 
                   break;              
               case 0:
                   break;
               default:
                   GestorMensagens.mostrarMensagem("\nOpcao invalida.");
                   break;
           }
       } catch (InputMismatchException e) {
           GestorMensagens.mostrarMensagem("Entrada invalida! Insira um numero.");
           s.nextLine(); // Limpa o buffer de entrada inválida
       }
   }

   /**
    * Aloca cidadãos em um terreno específico.
    *
    * Este método solicita ao jogador a quantidade de cidadãos a serem alocados 
    * e o ID do terreno onde os cidadãos serão alocados. Se a alocação for 
    * bem-sucedida, os cidadãos são alocados no terreno especificado.
    */
   private void opcaoAlocar() {
       GestorMensagens.mostrarMensagem("Quantos cidadaos: ");
       int qtdd = s.nextInt();
       if (qtdd <= 0) {
           GestorMensagens.mostrarMensagem("Valor insuficiente!");
           return;
       } else if (qtdd > cidade.getPopulacao().getPopDisponivel()) {
           GestorMensagens.mostrarMensagem("Valor superior ao numero de cidadaos disponiveis!");
           return;
       }

       GestorMensagens.mostrarMensagem("Escolha o ID do terreno: ");
       int id = s.nextInt();       

       ITerreno terreno = terrenosPorId.get(id); // Obtém o terreno pelo ID
       if (terreno == null) {
           GestorMensagens.mostrarMensagem("Terreno invalido. Tente novamente.");
           return;
       }

       cidade.alocarCidadao(terreno, qtdd); 
   }

   /**
    * Desaloca cidadãos de um terreno específico.
    *
    * Este método solicita ao jogador o ID do terreno de onde os cidadãos 
    * serão desalocados e a quantidade a ser desalocada. Se a operação for 
    * bem-sucedida, os cidadãos são removidos do terreno especificado.
    */
    private void opcaoDesalocar() {
        GestorMensagens.mostrarMensagem("Escolha o ID do terreno: ");
        int id = s.nextInt();

        ITerreno terreno = terrenosPorId.get(id); // Obtém o terreno pelo ID
        if (terreno == null) {
            GestorMensagens.mostrarMensagem("Id invalido!");
            return;
        } else if (!terreno.isTrabalhado()) {
            GestorMensagens.mostrarMensagem("Terreno sem trabalhadores!");
            return;
        }

        GestorMensagens.mostrarMensagem("Quantos cidadaos deseja desalocar: ");
        int qtdd = s.nextInt();

        cidade.desalocarCidadao(terreno, qtdd); 
    }
}
