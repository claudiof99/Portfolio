/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public class RecursoAlimento implements IRecurso{
    private int quantidade;

    public RecursoAlimento(int quantidade) {
        this.quantidade = quantidade;
    }

    /**
    * Obtém o tipo de recurso.
    *
    * Este método retorna o tipo do recurso, que neste caso é "Alimento".
    *
    * @return uma string representando o tipo de recurso
    */
   @Override
   public String getTipoRecurso() {
       return "Alimento";
   }

   /**
    * Obtém a quantidade do recurso.
    *
    * Este método retorna a quantidade atual do recurso disponível.
    *
    * @return um inteiro representando a quantidade do recurso
    */
   @Override
   public int getQuantidade() {
       return quantidade;
   }
}
