/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 */

package com.mycompany.poo.civilizations;
import java.util.*;
import java.io.*;

/**
 *
 * @author Diogo
 */
public class Civilizations {
    
    public static void main(String[] args) throws IOException{
       
        IMapa mapa = new MapaGeralRetangular(33, 15);   
        Civilizacao civiEscolhida = null;
        Scanner scan = new Scanner(System.in);
        
        iniciarCivilizacao(mapa, civiEscolhida, scan);
              
    }
    
    private static void iniciarCivilizacao(IMapa mapa, Civilizacao civilizacaoEscolhida, 
                                            Scanner scan) throws IOException{       
        int opcao;        
        // Lista de civilizações disponíveis
        ArrayList<Civilizacao> civilizacoes = new ArrayList<>();
        civilizacoes.add(new Civilizacao("Romana"));
        civilizacoes.add(new Civilizacao("Grega"));
        civilizacoes.add(new Civilizacao("Asteca"));
        civilizacoes.add(new Civilizacao("Persa"));
        civilizacoes.add(new Civilizacao("Egipcia"));

        IniciaCivilizacoes.carregarNomesCidades("nomes_cidades.txt");       
        // Exibição do menu para o jogador escolher uma civilização
        while (civilizacaoEscolhida == null) {
            System.out.println("-----------------Civilizacoes-----------------");           
            for (int i = 0; i < civilizacoes.size(); i++) {
                System.out.println((i + 1) + ". " + civilizacoes.get(i).getNome());
            }
            GestorMensagens.mostrarMensagem("0. Sair");
            GestorMensagens.mostrarMensagem("Escolha a sua Civilizacao:");
            try {
                opcao = scan.nextInt();

                if (opcao == 0) {
                    GestorMensagens.mostrarMensagem("\nSaindo do jogo.");
                    return;
                } else if (opcao > 0 && opcao <= civilizacoes.size()) {
                    civilizacaoEscolhida = civilizacoes.get(opcao - 1);
                    
                    GestorMensagens.setEstadoBloqueado();
                    // Distribuir civilizações aleatoriamente
                    IniciaCivilizacoes.distribuirCivilizacoes(mapa, civilizacoes, civilizacaoEscolhida);
                    GestorMensagens.setEstadoNormal();
                    
                    civilizacoes.remove(opcao - 1); // Remove o nome escolhido
                    IniciaCivilizacoes.iniciaCiviEscolhida(civilizacaoEscolhida);
                    civilizacaoEscolhida.associarCidades(mapa);
                    GestorMensagens.mostrarMensagem("\nEscolheu a civilizacao: " + 
                            civilizacaoEscolhida.getNome());
                    GestorMensagens.mostrarMensagem("******* Condicao de Vitoria *******");
                    GestorMensagens.mostrarMensagem("Conquistar todas as cidades inimigas" + 
                                                    "\n               ou" +
                                                    "\n   Atingir 1,000,000 de Ouro\n");
                } else {
                    GestorMensagens.mostrarMensagem("\nOpcao invalida. Tente novamente.");
                }
            } catch (InputMismatchException e) {
                GestorMensagens.mostrarMensagem("\nValor introduzido incorreto! Por favor, insira "
                        + "um numero inteiro.");
                scan.nextLine(); // Consome a entrada inválida
            }
        }
        testarMenu(mapa, civilizacaoEscolhida, civilizacoes, scan);  
    }
    
    private static void testarMenu(IMapa mapa, Civilizacao civilizacaoEscolhida, 
                                    ArrayList<Civilizacao> civilizacoes, Scanner scan) throws IOException{
        mapa.mostrarMapa();
        IMenu menuPrincipal = new MenuPrincipal(mapa, civilizacaoEscolhida, 
                civilizacoes); 
        int opcao = -1;
        // Exibição do menu principal do jogo
            scan.nextLine();
            menuPrincipal.processarEscolha(opcao);         
    }
}