/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;
import java.util.*;
import java.io.*;

/**
 *
 * @author Diogo
 */
public class IniciaCivilizacoes {
    private static final int NUM_CIDADES_INICIO = 3;
    
    private static Map<String, Queue<String>> nomesCidades;

    public IniciaCivilizacoes() {    
    }
    // Carrega os nomes das cidades a partir de um ficheiro
    public static Map<String, Queue<String>> carregarNomesCidades(String ficheiroNomes) {
        nomesCidades = new HashMap<>();

        try (BufferedReader br = new BufferedReader(new FileReader(ficheiroNomes))) {
            String linha;

            while ((linha = br.readLine()) != null) {
                String[] partes = linha.split(":");
                if (partes.length == 2) {
                    String civilizacao = partes[0].trim();
                    String[] nomesLista = partes[1].split(",");
                    Queue<String> filaNomes = new LinkedList<>();
                    for (String nome : nomesLista) {
                        filaNomes.add(nome.trim());
                    }
                    nomesCidades.put(civilizacao, filaNomes);
                }
            }
            br.close();
        } catch (IOException e) {
            System.err.println("Erro ao carregar o ficheiro de nomes: " + e.getMessage());
        }

        return nomesCidades;
    }
    // Distribui civilizações no mapa
    public static void distribuirCivilizacoes(IMapa mapa, ArrayList<Civilizacao> civilizacoes, 
            Civilizacao civilizacaoEscolhida) {
        Random rand = new Random();

        for (Civilizacao civilizacao : civilizacoes) {
            // Adicionar ouro aleatório
            civilizacao.adicionarOuro(rand.nextInt(50001) + 50000); // Tesouro entre 50,000 e 100,000
            // Obter fila de nomes associada à civilização
            Queue<String> filaNomes = nomesCidades.getOrDefault(civilizacao.getNome(), new LinkedList<>());

            for (int i = 0; i < NUM_CIDADES_INICIO; i++) { // Criar igual nº de cidades para todas
                int posX, posY;
                do {
                    posX = rand.nextInt(mapa.getLargura());
                    posY = rand.nextInt(mapa.getAltura());
                } while (!mapa.posicaoValidaParaCidade(posX, posY));

                // Obter o próximo nome da fila ou usar um nome padrão
                String nomeCidade = filaNomes.poll(); // Remove o elemento da fila
                if (nomeCidade == null) {
                    nomeCidade = "Cidade" + (i + 1) + " - " + civilizacao.getNome();
                }
                boolean inimigo = true;
                if (civilizacao.equals(civilizacaoEscolhida)) {inimigo = false;}
                // Adicionar cidade ao mapa
                mapa.adicionarCidade(civilizacao, nomeCidade, posX, posY, inimigo);
            }
        }
    }
    
    public static void iniciaCiviEscolhida(Civilizacao civilizacao) {
        Random rand = new Random();
        
        for (Cidade cidade : civilizacao.getCidades()) { // Iterar sobre as cidades da civilização
            cidade.getPopulacao().setTotal(rand.nextInt(901) + 100); // População entre 100 e 1000
        }
        civilizacao.setTesouroOuro(rand.nextInt(10001) + 10000); // Tesouro entre 10,000 e 20,000       
    }
}
