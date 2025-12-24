/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public interface IUnidade extends IAtacavel{
    void mover(IMapa mapa, int x, int y); // Movimenta a unidade para coordenadas (x, y)
    int getCustoManutencao(); // Retorna o custo de manutenção (0 para unidades sem custo)
    String getSimbolo(); // Retorna o símbolo visual da unidade
    boolean isOcupada(); // Retorna se a unidade está ocupada
    void setOcupada(boolean ocupada); // Define o estado da unidade  
    void acao(IAtacavel alvo);
    Cidade getCidade();
    void setCidade(Cidade cidade);
}
