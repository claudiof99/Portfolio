/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public interface IRecurso {
    String getTipoRecurso(); // Retorna o tipo de recurso: "comida", "ouro", "produção industrial"
    int getQuantidade(); // Retorna a quantidade de recurso gerado
}
