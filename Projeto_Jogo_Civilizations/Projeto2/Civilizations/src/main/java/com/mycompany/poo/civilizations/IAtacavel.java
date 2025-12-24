/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public interface IAtacavel {
    void receberDano(int quantidade, Civilizacao atacante);
    int getDefesa();
    void setDefesa(int defesa);
    boolean estaDerrotada();
    String getDescricao();
    int getForcaCombate();
    int getPosX();
    int getPosY();
}
