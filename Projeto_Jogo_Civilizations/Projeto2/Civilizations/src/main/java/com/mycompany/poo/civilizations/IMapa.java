/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.util.ArrayList;

/**
 *
 * @author Diogo
 */
public interface IMapa {
    ITerreno getTerreno(int x, int y); // Retorna o tipo de terreno numa posição específica
    void setTerreno(int x, int y, ITerreno terreno);
    ArrayList<ITerreno> getTerrenosProximos(int posX, int posY, int distanciaMaxima);
    void mostrarMapa(); // Exibe o mapa na consola
    void adicionarCidade(Civilizacao civilizacao, String nome, int posX, int posY, boolean inimigo);
    boolean existemCidades();
    void mostrarMapaZoom(int centroX, int centroY, int raio);
    ArrayList<Cidade> getCidades();
    boolean posicaoValida(int posX, int posY);
    boolean posicaoValidaParaCidade(int posX, int posY);
    void iniTerrenosMapa(ITerreno[][] mapa, int largura, int altura);
    int getAltura();
    int getLargura();
    Cidade getCidadePorCoordenada(Civilizacao civilizacao, int x, int y);
}
