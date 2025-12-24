/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.ArrayList;

/**
 *
 * @author Diogo
 */
public class Turno {
    private IMapa mapa;
    private static int numeroTurno;

    public Turno(IMapa mapa) {
        this.mapa = mapa;
        numeroTurno = 1;
    }
    /**
     * Inicia o turno para a civilização especificada.
     *
     * Este método verifica as condições de vitória, processa as ações 
     * planejadas de movimento e construção, e atualiza os recursos 
     * das cidades no início do turno. Se a civilização atingir 
     * 1.000.000 em ouro ou eliminar todas as cidades inimigas, 
     * o jogo é encerrado.
     *
     * @param civilizacao a civilização que está jogando
     * @param movimento o gestor de movimento para processar movimentos
     * @param ataque o gestor de ataque (não utilizado neste método)
     * @param construcao o gestor de construção para processar construções
     */
    public void iniciarTurno(Civilizacao civilizacao, GestorMovimento movimento, 
            GestorAtaque ataque, GestorConstrucao construcao) {
        if (verificaVitoriaOuro(civilizacao)) {
            GestorMensagens.mostrarMensagem("\nParabens! Concluiste o jogo. Conseguiste chegar aos 1,000,000 em ouro.");
            System.exit(0);
        }       
        if (verificaVitoriaCidadesInimigas(civilizacao)) {
            GestorMensagens.mostrarMensagem("\nParabens! Concluiste o jogo. Conseguiste eliminar todas as cidades inimigas.");
            System.exit(0);
        }
        GestorMensagens.mostrarMensagem("\n--- Turno " + numeroTurno + " ---\n");
        // Processar ações planeadas
        movimento.processarMovimentos();
        construcao.processarConstrucoes();
        // Limpar recursos no início do turno
        for (Cidade cidade : civilizacao.getCidades()) {
            civilizacao.adicionarOuro(cidade.getGestorRecursos().getSobraOuro());            
            cidade.getGestorRecursos().resetProducao(cidade);
        }
        for (Cidade cidade : civilizacao.getCidades()) {           
            cidade.necessidadesEProducao(); // Alimenta a população  
            cidade.getGestorUnidades().aplicarCustoManutencao(); // Cobra a manutenção das unidades
        }
        numeroTurno++;
    }

    /**
     * Verifica se a civilização atingiu a vitória por ouro.
     *
     * Este método verifica se a quantidade de ouro da civilização 
     * é igual ou superior a 1.000.000.
     *
     * @param civilizacao a civilização a ser verificada
     * @return true se a civilização atingiu a vitória por ouro, false caso contrário
     */
    public boolean verificaVitoriaOuro(Civilizacao civilizacao) {  
        return civilizacao.getTesouroOuro() >= 1000000;
    }

    /**
     * Verifica se a civilização eliminou todas as cidades inimigas.
     *
     * Este método conta o número de cidades inimigas restantes e 
     * verifica se esse número é zero.
     *
     * @param civilizacao a civilização a ser verificada
     * @return true se todas as cidades inimigas foram eliminadas, false caso contrário
     */
    public boolean verificaVitoriaCidadesInimigas(Civilizacao civilizacao) {
        ArrayList<Cidade> todasAsCidades = mapa.getCidades();       
        return civilizacao.contarCidadesInimigas(civilizacao, todasAsCidades) == 0;
    }

    /**
     * Obtém o número atual do turno.
     *
     * Este método retorna o número do turno atual.
     *
     * @return o número do turno atual
     */
    public static int getNumTurno() {
        return numeroTurno;
    }

    /**
     * Define o número do turno.
     *
     * Este método atualiza o número do turno com o valor fornecido.
     *
     * @param numeroTurno o novo número do turno a ser definido
     */
    public static void setNumTurno(int numeroTurno) {
        Turno.numeroTurno = numeroTurno;
    }
}
