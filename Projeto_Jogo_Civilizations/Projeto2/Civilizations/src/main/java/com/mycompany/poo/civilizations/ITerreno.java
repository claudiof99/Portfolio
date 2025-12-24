/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.util.ArrayList;
import java.util.Stack;

/**
 *
 * @author Diogo
 */
public interface ITerreno { 
    final String SIMBOLO_ESTRADA = " E";
    final int CUSTO_INACESSIVEL = -1;
    
    void adicionarAlvo(IAtacavel alvo);
    void removerAlvo(IAtacavel alvo);
    ArrayList<IAtacavel> getAlvos();
    boolean temAlvo();
    boolean eAcessivel(); // Retorna se o terreno permite movimentação
    int getCustoMovimento(); // Custo de movimento para o tipo de terreno
    void setCustoMovimento(int custo);
    String getSimbolo(); // Símbolo para visualização na consola
    ArrayList<IRecurso> getRecursos(); // Retorna a lista de recursos disponíveis
    String getRecursosResumo();
    void adicionarRecurso(IRecurso recurso); // Adiciona um recurso ao terreno
    boolean isTrabalhado();
    int getCidadaosTrabalhando(); // Retorna o número de cidadãos alocados
    void adicionarCidadaos(int quantidade); // Adiciona cidadãos ao terreno
    void removerCidadaos(int quantidade);   // Remove cidadãos do terreno
    int getX();
    int getY();
    void clearOverlaySimbolo(String simbolo);
    void setOverlaySimbolo(String simbolo);
    String getOverlaySimbolo();
    boolean procuraOverlay(String simbolo);
    Stack<String> getOverlays();
    void setOverlays(Stack<String> overlays);
}
