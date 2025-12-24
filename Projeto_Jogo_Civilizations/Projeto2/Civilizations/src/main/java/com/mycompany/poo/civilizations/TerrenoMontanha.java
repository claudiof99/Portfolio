/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.ArrayList;
import java.util.Stack;

/**
 *
 * @author Diogo
 */
public class TerrenoMontanha implements ITerreno{
    private final String SIMBOLO = "M"; // Representa a montanha no mapa
    
    private ArrayList<IRecurso> recursos;
    private ArrayList<IAtacavel> alvos;
    private String simbolo;
    private int cidadaosTrabalhando, x, y, custoMovimento;
    private Stack<String> overlays; // Para gerir múltiplos símbolos
    
    public TerrenoMontanha(int x, int y) {
        recursos = new ArrayList<>();
        alvos = new ArrayList<>();
        recursos.add(new RecursoOuro(60)); // Exemplo de inicialização com Minerio
        recursos.add(new RecursoProducao(80)); // Inicializa com Materia Prima
        recursos.add(new RecursoAlimento(40)); // Inicializa com Alimento
        this.cidadaosTrabalhando = 0;
        this.custoMovimento = 3;
        this.x = x;
        this.y = y;
        this.simbolo = SIMBOLO;
        this.overlays = new Stack<>();
    }
    /**
    * Adiciona um alvo à lista de alvos.
    *
    * Este método verifica se o alvo já está na lista de alvos. Se não estiver, 
    * ele é adicionado.
    *
    * @param alvo o alvo a ser adicionado
    */
   @Override
   public void adicionarAlvo(IAtacavel alvo) {
       if (!alvos.contains(alvo)) {
           alvos.add(alvo);
       }
   }

   /**
    * Remove um alvo da lista de alvos.
    *
    * Este método verifica se o alvo está na lista de alvos. Se estiver, 
    * ele é removido.
    *
    * @param alvo o alvo a ser removido
    */
   @Override
   public void removerAlvo(IAtacavel alvo) {
       if (alvos.contains(alvo)) {
           alvos.remove(alvo);
       }
   }

   /**
    * Obtém a lista de alvos.
    *
    * Este método retorna uma nova lista contendo todos os alvos atuais.
    *
    * @return uma lista de alvos
    */
   @Override
   public ArrayList<IAtacavel> getAlvos() {
       return new ArrayList<>(alvos);
   }

   /**
    * Verifica se existem alvos na lista.
    *
    * Este método retorna true se houver pelo menos um alvo na lista; 
    * caso contrário, retorna false.
    *
    * @return true se houver alvos, false caso contrário
    */
   @Override
   public boolean temAlvo() {
       return !alvos.isEmpty();
   }

   /**
    * Obtém a coordenada X do objeto.
    *
    * @return a coordenada X
    */
   @Override
   public int getX() {
       return x;
   }

   /**
    * Obtém a coordenada Y do objeto.
    *
    * @return a coordenada Y
    */
   @Override
   public int getY() {
       return y;
   }

   /**
    * Verifica se o terreno é acessível.
    *
    * Este método sempre retorna true, indicando que o terreno é acessível.
    *
    * @return true, pois o terreno é acessível
    */
   @Override
   public boolean eAcessivel() {
       return true;
   }

   /**
    * Obtém o custo de movimento para atravessar o terreno.
    *
    * Este método retorna o custo de movimento associado ao terreno.
    *
    * @return o custo de movimento
    */
   @Override
   public int getCustoMovimento() {
       return custoMovimento; // Maior custo para áreas montanhosas
   }

   /**
    * Define o custo de movimento para o terreno.
    *
    * Este método atualiza o custo de movimento do terreno, permitindo 
    * ajustes conforme necessário.
    *
    * @param custoMovimento o novo custo de movimento
    */
   @Override
   public void setCustoMovimento(int custoMovimento) {
       this.custoMovimento = custoMovimento; // Menor custo para áreas planas
   }
    /**
     * Obtém o símbolo representativo do objeto.
     *
     * Este método retorna o símbolo do objeto, priorizando o símbolo do 
     * overlay, se existir. Se o terreno estiver sendo trabalhado, um 
     * símbolo destacado é retornado.
     *
     * @return o símbolo do objeto
     */
    @Override
    public String getSimbolo() {
        if (!overlays.isEmpty()) {
            return getOverlaySimbolo(); // Prioriza o símbolo do overlay
        }
        // Retorna um símbolo destacado se o terreno estiver sendo trabalhado
        return isTrabalhado() ? "*" + simbolo : " " + simbolo;
    }

    /**
     * Verifica se o terreno está sendo trabalhado.
     *
     * Este método retorna true se houver trabalhadores alocados no terreno; 
     * caso contrário, retorna false.
     *
     * @return true se o terreno estiver sendo trabalhado, false caso contrário
     */
    @Override
    public boolean isTrabalhado() {
        return cidadaosTrabalhando > 0;
    }

    /**
     * Obtém o número de cidadãos trabalhando no terreno.
     *
     * @return o número de cidadãos trabalhando
     */
    @Override
    public int getCidadaosTrabalhando() {
        return cidadaosTrabalhando;
    }

    /**
     * Adiciona cidadãos ao número de trabalhadores no terreno.
     *
     * Este método incrementa o número de cidadãos trabalhando no terreno 
     * pelo valor especificado.
     *
     * @param quantidade o número de cidadãos a serem adicionados
     */
    @Override
    public void adicionarCidadaos(int quantidade) {
        cidadaosTrabalhando += quantidade;
    }

    /**
     * Remove cidadãos do número de trabalhadores no terreno.
     *
     * Este método decrementa o número de cidadãos trabalhando no terreno 
     * pelo valor especificado, garantindo que o número não fique negativo.
     *
     * @param quantidade o número de cidadãos a serem removidos
     */
    @Override
    public void removerCidadaos(int quantidade) {
        cidadaosTrabalhando = Math.max(0, cidadaosTrabalhando - quantidade);
    }

    /**
     * Obtém a lista de recursos disponíveis.
     *
     * Este método retorna uma nova lista contendo todos os recursos 
     * associados ao objeto.
     *
     * @return uma lista de recursos
     */
    @Override
    public ArrayList<IRecurso> getRecursos() {
        return new ArrayList<IRecurso>(recursos);
    }

    /**
     * Obtém um resumo dos recursos disponíveis.
     *
     * Este método cria uma string que resume todos os recursos, incluindo 
     * o tipo e a quantidade de cada um. Os recursos são separados por vírgulas.
     *
     * @return uma string representando o resumo dos recursos
     */
    @Override
    public String getRecursosResumo() {
        StringBuilder resumo = new StringBuilder();
        for (IRecurso recurso : recursos) {
            if (resumo.length() > 0) {
                resumo.append(", "); // Adiciona separador entre os recursos
            }
            resumo.append(recurso.getTipoRecurso())
                  .append(": ")
                  .append(recurso.getQuantidade());
        }
        return resumo.toString();
    }

    /**
     * Adiciona um recurso à lista de recursos do objeto.
     *
     * Este método adiciona o recurso especificado à lista de recursos 
     * disponíveis.
     *
     * @param recurso o recurso a ser adicionado
     */
    @Override
    public void adicionarRecurso(IRecurso recurso) {
        recursos.add(recurso);
    }

    /**
     * Define um símbolo de overlay para o objeto.
     *
     * Este método adiciona um símbolo à pilha de overlays. Se o símbolo 
     * for do tipo "estrada", o custo de movimento é ajustado para 1.
     *
     * @param simbolo o símbolo a ser adicionado como overlay
     */
    @Override
    public void setOverlaySimbolo(String simbolo) {
        overlays.push(simbolo);
        if (simbolo.equals(SIMBOLO_ESTRADA)) {
            custoMovimento = 1;
        }
    }

    /**
     * Remove um símbolo de overlay do objeto.
     *
     * Este método remove o símbolo especificado da pilha de overlays.
     *
     * @param simbolo o símbolo a ser removido
     */
    @Override
    public void clearOverlaySimbolo(String simbolo) {
        overlays.remove(simbolo);
    }

    /**
     * Obtém o símbolo do overlay mais recente.
     *
     * Este método retorna o símbolo do overlay no topo da pilha. Se a pilha 
     * estiver vazia, retorna uma string vazia.
     *
     * @return o símbolo do overlay mais recente ou uma string vazia se não houver overlays
     */
    @Override
    public String getOverlaySimbolo() {
        return overlays.isEmpty() ? "" : overlays.peek();
    }

    /**
     * Verifica se um símbolo específico existe na pilha de overlays.
     *
     * Este método retorna true se o símbolo estiver presente na pilha; 
     * caso contrário, retorna false.
     *
     * @param simbolo o símbolo a ser verificado
     * @return true se o símbolo estiver
     */
    @Override
    public boolean procuraOverlay(String simbolo) {
        return overlays.contains(simbolo);
    }
    /**
    * Obtém a pilha de overlays.
    *
    * Este método retorna a pilha de símbolos de overlay associados ao objeto.
    *
    * @return a pilha de overlays
    */
   @Override
   public Stack<String> getOverlays() {
       return overlays;
   }

   /**
    * Define a pilha de overlays para o objeto.
    *
    * Este método atualiza a pilha de overlays com a pilha fornecida.
    *
    * @param overlays a nova pilha de overlays a ser definida
    */
   @Override
   public void setOverlays(Stack<String> overlays) {
       this.overlays = overlays;
   }

   /**
    * Retorna uma representação em string do objeto.
    *
    * Este método retorna uma string que representa o objeto, neste caso, 
    * o tipo de recurso "Montanha".
    *
    * @return uma string representando o objeto
    */
   @Override
   public String toString() {
       return "Montanha";
   }
}
