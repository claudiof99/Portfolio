/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 * Representa uma agenda de ação associada a uma unidade no jogo Civilizations.
 * A agenda define uma unidade e as coordenadas de destino para uma ação, como movimentação ou construção.
 * 
 * @author Diogo
 */
public class AgendaAcao {
    //A unidade associada à ação.
    private IUnidade unidade;
    //A coordenada X do destino da ação.
    private int destinoX;
    //A coordenada Y do destino da ação.
    private int destinoY;
    /**
     * Construtor da classe AgendaAcao. Inicializa uma nova agenda de ação com a unidade
     * e as coordenadas de destino especificadas.
     * 
     * @param unidade a unidade que realizará a ação.
     * @param destinoX a coordenada X do destino da ação.
     * @param destinoY a coordenada Y do destino da ação.
     */
    public AgendaAcao(IUnidade unidade, int destinoX, int destinoY) {
        this.unidade = unidade;
        this.destinoX = destinoX;
        this.destinoY = destinoY;
    }

    /**
     * Obtém a unidade associada à ação.
     * 
     * @return a unidade que realizará a ação.
     */
    public IUnidade getUnidade() {
        return unidade;
    }

    /**
     * Obtém a coordenada X do destino da ação.
     * 
     * @return a coordenada X do destino.
     */
    public int getDestinoX() {
        return destinoX;
    }

    /**
     * Obtém a coordenada Y do destino da ação.
     * 
     * @return a coordenada Y do destino.
     */
    public int getDestinoY() {
        return destinoY;
    }
}

