/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public class Populacao {
    private int total, trabalhadores, disponivel;
    private Cidade cidade;

    // Construtor
    public Populacao(Cidade cidade, int totalInicial) {
        this.cidade = cidade;
        this.total = totalInicial;
        this.disponivel = totalInicial;
        this.trabalhadores = 0;
    }
    /**
    * Obtém o total de cidadãos na população.
    *
    * @return o número total de cidadãos
    */
   public int getTotal() {
       return total;
   }

   /**
    * Define o total de cidadãos na população.
    *
    * Este método atualiza o total de cidadãos e também define o número 
    * de cidadãos disponíveis como igual ao total.
    *
    * @param total o novo total de cidadãos
    */
   public void setTotal(int total) {
       this.total = total;
       disponivel = total;
   }

   /**
    * Adiciona um valor ao total de cidadãos na população.
    *
    * Este método incrementa o total de cidadãos e também o número de 
    * cidadãos disponíveis.
    *
    * @param valor o valor a ser adicionado ao total de cidadãos
    */
   public void addTotal(int valor) {
       total += valor;
       disponivel += valor;
   }

   /**
    * Obtém o número de trabalhadores na população.
    *
    * @return o número de trabalhadores
    */
   public int getTrabalhadores() {
       return trabalhadores;
   }

   /**
    * Define o número de trabalhadores na população.
    *
    * Este método incrementa o número de trabalhadores e decrementa o 
    * número de cidadãos disponíveis.
    *
    * @param valor o valor a ser adicionado ao número de trabalhadores
    */
   public void setTrabalhadores(int valor) {
       trabalhadores += valor;
       disponivel -= valor;
   }

   /**
    * Obtém o número de cidadãos disponíveis na população.
    *
    * @return o número de cidadãos disponíveis
    */
   public int getPopDisponivel() {
       return disponivel;
   }

   /**
    * Remove um cidadão da população, priorizando a remoção de não trabalhadores.
    *
    * Este método decrementa o número de cidadãos disponíveis. Se não houver 
    * cidadãos disponíveis, um cidadão é desalocado de um terreno trabalhado 
    * na cidade.
    */
   public void removerCidadao() {
       if (disponivel > 0) {
           addTotal(-1);
       } else {
           cidade.desalocarCidadao(cidade.getTerrenosTrabalhados().get(0), 1);
           total--;
       }
   }
}
