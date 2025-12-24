/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.*;

/**
 *
 * @author Diogo
 */
public class GestorUnidades {
    private Cidade cidade;
    private IMapa mapa;
    private HashMap<String, List<IUnidade>> unidades;
    private HashMap<String, Integer> custosProducao;

    public GestorUnidades(Cidade cidade, IMapa mapa) {
        this.cidade = cidade;
        this.mapa = mapa;
        this.unidades = new HashMap<>();
        this.custosProducao = new HashMap<>(); 
        inicializarTiposUnidades();
    }
    /**
    * Inicializa os tipos de unidades e seus custos de produção.
    *
    * Este método configura as listas de unidades para cada tipo (CONSTRUTOR, 
    * MILITAR e COLONO) e define os custos de produção correspondentes para 
    * cada tipo de unidade.
    */
   private void inicializarTiposUnidades() {
       unidades.put("CONSTRUTOR", new ArrayList<>());
       unidades.put("MILITAR", new ArrayList<>());
       unidades.put("COLONO", new ArrayList<>());
       // Define os custos de produção para cada tipo
       custosProducao.put("CONSTRUTOR", 30);
       custosProducao.put("MILITAR", 50);
       custosProducao.put("COLONO", 70);
   }

   /**
    * Verifica se existe pelo menos uma unidade do tipo especificado na cidade.
    *
    * Este método verifica se o tipo de unidade fornecido é válido e se há 
    * unidades desse tipo disponíveis na cidade.
    *
    * @param tipo O tipo de unidade a verificar.
    * @return True se existir pelo menos uma unidade do tipo especificado, 
    *         caso contrário, False.
    */
   public boolean verificarUnidadeExistente(String tipo) {
       // Verifica se o tipo é válido
       if (!unidades.containsKey(tipo)) {
           GestorMensagens.mostrarMensagem("Tipo de unidade invalido: " + tipo);
           return false;
       }
       // Verifica se há unidades do tipo especificado
       List<IUnidade> listaUnidades = unidades.get(tipo);
       return listaUnidades != null && !listaUnidades.isEmpty();
   }

   /**
    * Produz uma unidade do tipo especificado, se possível.
    *
    * Este método verifica se o tipo de unidade é válido e se há recursos 
    * suficientes para produzir a unidade. Se a produção for bem-sucedida, 
    * a nova unidade é criada e adicionada à lista de unidades.
    *
    * @param tipo o tipo da unidade a ser produzida
    */
   public void produzirUnidade(String tipo) {
       // Verificar se o tipo é válido
       if (!custosProducao.containsKey(tipo)) {
           GestorMensagens.mostrarMensagem("Tipo de unidade invalido: " + tipo);
           return;
       }        
       int custo = custosProducao.get(tipo);
       if (cidade.getGestorRecursos().getQuantidade("Producao") < custo) {
           GestorMensagens.mostrarMensagem("Producao insuficiente para criar unidade " + tipo);
           return;
       }
       // Subtrai o custo da produção da cidade
       cidade.getGestorRecursos().consumirRecurso("Producao", custo);

       IUnidade novaUnidade = null;
       // Criar a unidade de acordo com o tipo
       switch (tipo) {
           case "CONSTRUTOR":
               novaUnidade = new UnidadeConstrutor(cidade.getPosX(), cidade.getPosY(), cidade);
               break;
           case "MILITAR":
               novaUnidade = new UnidadeMilitar(cidade.getPosX(), cidade.getPosY(), cidade);
               break;
           case "COLONO":
               novaUnidade = new UnidadeColono(cidade.getPosX(), cidade.getPosY(), cidade);
               break;
           default:
               GestorMensagens.mostrarMensagem("Tipo de unidade invalido: " + tipo);
               return;
       }
       // Adicionar a nova unidade à coleção
       unidades.get(tipo).add(novaUnidade);
       GestorMensagens.mostrarMensagem("Unidade " + tipo + " produzida na cidade " + 
               cidade.getNome());
   }

   /**
    * Aplica o custo de manutenção das unidades na cidade.
    *
    * Este método calcula o custo total de manutenção de todas as unidades 
    * e verifica se há recursos suficientes para cobrir esse custo. Se não 
    * houver, unidades são removidas até que o custo seja equilibrado.
    */
    public void aplicarCustoManutencao() {
        int ouroDisponivel = cidade.getGestorRecursos().getQuantidade("Ouro");
        int totalCustoManutencao = 0;
        // Verifica se todas as listas de unidades estão vazias
        boolean haUnidades = unidades.values().stream().anyMatch(lista -> !lista.isEmpty());
        
        if (haUnidades){
            // Calcular o custo total de manutenção
            for (List<IUnidade> listaUnidades : unidades.values()) {
                for (IUnidade unidade : listaUnidades) {
                    totalCustoManutencao += unidade.getCustoManutencao();
                }
            }
            if (ouroDisponivel >= totalCustoManutencao) {
                // Ouro suficiente para pagar a manutenção
                cidade.getGestorRecursos().consumirRecurso("Ouro", totalCustoManutencao);
                GestorMensagens.mostrarMensagem("Todos os custos de manutencao das unidades foram cobrados!");
            } else {
                // Remover unidades até equilibrar o custo
                int ouroRestante = ouroDisponivel;
                for (String tipo : unidades.keySet()) {
                    List<IUnidade> listaUnidades = unidades.get(tipo);
                    for (int i = 0; i < listaUnidades.size(); i++) {
                        IUnidade unidade = listaUnidades.get(i);
                        if (ouroRestante >= unidade.getCustoManutencao()) {
                            ouroRestante -= unidade.getCustoManutencao();
                            cidade.getGestorRecursos().consumirRecurso("Ouro", unidade.getCustoManutencao());
                        } else {
                            // Remover unidade
                            listaUnidades.remove(i);
                            i--; // Ajustar índice
                            GestorMensagens.mostrarMensagem("Unidade do tipo " + tipo + 
                                    " removida por falta de ouro na cidade " + cidade.getNome());
                        }
                    }
                }           
            }
        }
        
    }

    /**
    * Consome uma unidade do tipo especificado.
    *
    * Este método remove a primeira unidade da lista de unidades do tipo 
    * especificado. Se não houver unidades disponíveis desse tipo, uma 
    * mensagem de erro é exibida.
    *
    * @param tipo o tipo da unidade a ser consumida
    */
   public void consumirUnidade(String tipo) {
       List<IUnidade> listaUnidades = unidades.get(tipo);

       if (listaUnidades != null && !listaUnidades.isEmpty()) {
           listaUnidades.remove(0); // Remove a primeira unidade da lista
       } else {
           GestorMensagens.mostrarMensagem("Nao ha unidades disponiveis do tipo " + 
                   tipo + " na cidade " + cidade.getNome());
       }
   }

   /**
    * Exibe a quantidade de unidades disponíveis na cidade.
    *
    * Este método exibe uma mensagem com a quantidade de unidades de cada tipo 
    * que estão presentes na cidade.
    */
   public void exibirUnidades() {
       GestorMensagens.mostrarMensagem("Unidades na cidade " + cidade.getNome() + ":");
       for (String tipo : unidades.keySet()) {
           GestorMensagens.mostrarMensagem("  - " + tipo + ": " + unidades.get(tipo).size() + " unidades");
       }
   }

   /**
    * Obtém uma lista de colonos que estão na cidade.
    *
    * Este método verifica todos os colonos e retorna uma lista daqueles que 
    * estão localizados em terrenos do tipo "Cidade" e cuja população total 
    * é zero.
    *
    * @return uma lista de colonos que estão na cidade
    */
   public List<IUnidade> getColonosEmCidade() {
       List<IUnidade> colonosEmCidade = new ArrayList<>();
       for (IUnidade colono : unidades.get("COLONO")) {
           if (mapa.getTerreno(colono.getPosX(), colono.getPosY()).toString().equals("Cidade")) {
               Cidade cidade = ((ITerrenoCidade) mapa.getTerreno(colono.getPosX(), 
                       colono.getPosY())).getCidade();
               if (cidade.getPopulacao().getTotal() == 0) {
                   colonosEmCidade.add(colono);
               }
           }
       }
       return colonosEmCidade;
   }

   /**
    * Obtém um mapa contendo todas as unidades organizadas por tipo.
    *
    * Este método retorna um HashMap onde as chaves são os tipos de unidades 
    * e os valores são listas de unidades correspondentes.
    *
    * @return um HashMap contendo as unidades organizadas por tipo
    */
   public HashMap<String, List<IUnidade>> getUnidades() {
       return unidades;
   }

   /**
    * Coloniza uma cidade utilizando um colono.
    *
    * Este método aumenta a população da cidade em 10 e define a defesa da 
    * cidade como 200. Após a colonização, a unidade colono utilizada é 
    * removida da lista de unidades.
    *
    * @param colono a unidade colono que está colonizando a cidade
    * @param cidade a cidade a ser colonizada
    */
   public void colonizar(IUnidade colono, Cidade cidade) {        
       cidade.getPopulacao().addTotal(10);
       cidade.setDefesa(200);
       GestorMensagens.mostrarMensagem("A cidade " + cidade.getNome() + " foi colonizada.");
       // Remove a unidade Colono
       unidades.get("COLONO").remove(colono);
   }
}
