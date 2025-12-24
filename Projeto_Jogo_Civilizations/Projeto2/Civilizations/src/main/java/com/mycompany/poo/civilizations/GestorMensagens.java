/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public class GestorMensagens {
    private static enum Estado { NORMAL, BLOQUEADO }

    private static Estado estadoAtual = Estado.NORMAL;

    /**
    * Define o estado atual como bloqueado.
    *
    * Este método altera o estado atual do sistema para o estado BLOQUEADO, 
    * impedindo que mensagens sejam exibidas.
    */
   public static void setEstadoBloqueado() {
       estadoAtual = Estado.BLOQUEADO;
   }

   /**
    * Define o estado atual como normal.
    *
    * Este método altera o estado atual do sistema para o estado NORMAL, 
    * permitindo que mensagens sejam exibidas.
    */
   public static void setEstadoNormal() {
       estadoAtual = Estado.NORMAL;
   }

   /**
    * Exibe uma mensagem no console se o estado atual for NORMAL.
    *
    * Este método verifica o estado atual e, se for NORMAL, imprime a mensagem 
    * fornecida no console.
    *
    * @param mensagem a mensagem a ser exibida
    */
   public static void mostrarMensagem(Object mensagem) {
       if (estadoAtual == Estado.NORMAL) {
           System.out.println(mensagem);
       }
   }

   /**
    * Exibe uma mensagem formatada no console se o estado atual for NORMAL.
    *
    * Este método verifica o estado atual e, se for NORMAL, imprime a mensagem 
    * formatada no console, utilizando o formato especificado e os argumentos 
    * fornecidos.
    *
    * @param formato a string de formato
    * @param args os argumentos a serem formatados na string
    */
   public static void mostrarFormatado(String formato, Object... args) {
       if (estadoAtual == Estado.NORMAL) {
           System.out.printf(formato, args);
       }
   }
}
