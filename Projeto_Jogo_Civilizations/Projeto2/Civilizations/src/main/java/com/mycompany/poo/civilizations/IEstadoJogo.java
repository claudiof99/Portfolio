/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Interface.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.io.*;

/**
 *
 * @author Diogo
 */
public interface IEstadoJogo {
    void gravaDados(BufferedWriter writer) throws IOException;
    Object carregarEstado(BufferedReader reader, IMapa mapa, boolean jogador) throws IOException;
}
