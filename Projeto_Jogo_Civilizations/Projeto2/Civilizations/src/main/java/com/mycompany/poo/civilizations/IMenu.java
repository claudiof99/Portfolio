/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.io.IOException;
/**
 *
 * @author Diogo
 */
public interface IMenu {
    void exibirOpcoes();       // Exibe as opções disponíveis no menu
    void processarEscolha(int opcao) throws IOException; // Processa a escolha do jogador
}
