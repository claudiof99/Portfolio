/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

/**
 *
 * @author Diogo
 */
public class UnidadeColono implements IUnidade, IAtacavel{
    private int posX, posY;
    private int vida;
    private int forcaCombate;
    private int custoManutencao;
    private boolean ocupada;
    private Cidade cidade;

    public UnidadeColono(int x, int y, Cidade cidade) {
        this.posX = x;
        this.posY = y;
        this.forcaCombate = 1;
        this.vida = 1;
        this.custoManutencao = 0;
        this.ocupada = false; // Inicialmente livre
        this.cidade = cidade;
    }
    /**
     * Obtém a força de combate do objeto.
     *
     * Este método retorna o valor da força de combate.
     *
     * @return a força de combate
     */
    @Override
    public int getForcaCombate() {
        return forcaCombate;
    }

    /**
     * Obtém a defesa do objeto.
     *
     * Este método retorna o valor da vida, que representa a defesa.
     *
     * @return a defesa (vida)
     */
    @Override
    public int getDefesa() {
        return vida;
    }

    /**
     * Define a defesa do objeto.
     *
     * Este método atualiza o valor da vida, que representa a defesa.
     *
     * @param vida o novo valor de defesa (vida)
     */
    @Override
    public void setDefesa(int vida) {
        this.vida = vida;
    }

    /**
     * Recebe dano no objeto.
     *
     * Este método reduz a vida do objeto pelo valor de dano recebido. 
     * Se a vida ficar abaixo de zero, ela é ajustada para zero e uma 
     * mensagem de derrota é exibida.
     *
     * @param dano o valor de dano a ser recebido
     * @param atacante a civilização que está atacando
     */
    @Override
    public void receberDano(int dano, Civilizacao atacante) {
        this.vida -= dano;
        if (this.vida < 0) {
            this.vida = 0;
            GestorMensagens.mostrarMensagem(this + " foi derrotado!");
        }
    }

    /**
     * Verifica se o objeto está derrotado.
     *
     * Este método retorna true se a vida do objeto for menor ou igual a zero; 
     * caso contrário, retorna false.
     *
     * @return true se o objeto estiver derrotado, false caso contrário
     */
    @Override
    public boolean estaDerrotada() {
        return this.vida <= 0;
    }

    /**
     * Verifica se o objeto está ocupado.
     *
     * Este método retorna o estado de ocupação do objeto.
     *
     * @return true se o objeto estiver ocupado, false caso contrário
     */
    @Override
    public boolean isOcupada() {
        return ocupada;
    }

    /**
     * Define o estado de ocupação do objeto.
     *
     * Este método atualiza o estado de ocupação do objeto.
     *
     * @param ocupada o novo estado de ocupação
     */
    @Override
    public void setOcupada(boolean ocupada) {
        this.ocupada = ocupada;
    }

    /**
     * Obtém a cidade associada ao objeto.
     *
     * Este método retorna a cidade à qual o objeto está associado.
     *
     * @return a cidade associada
     */
    @Override
    public Cidade getCidade() {
        return cidade;
    }

    /**
     * Define a cidade associada ao objeto.
     *
     * Este método atualiza a cidade à qual o objeto está associado.
     *
     * @param cidade a nova cidade a ser associada
     */
    @Override
    public void setCidade(Cidade cidade) {
        this.cidade = cidade;
    }

    /**
     * Move o objeto para uma nova posição no mapa.
     *
     * Este método atualiza a posição do objeto e adiciona o símbolo 
     * ao novo terreno. Se o novo terreno for uma cidade, o símbolo 
     * não é adicionado.
     *
     * @param mapa o mapa onde o objeto será movido
     * @param x a nova coordenada X
     * @param y a nova coordenada Y
     */
    @Override
    public void mover(IMapa mapa, int x, int y) {
        // Atualizar posição
        this.posX = x;
        this.posY = y;
        // Adicionar ao novo terreno
        ITerreno novoTerreno = mapa.getTerreno(x, y);
        if (!(novoTerreno instanceof TerrenoCidade)) {
            novoTerreno.setOverlaySimbolo(this.getSimbolo());
        } 
        GestorMensagens.mostrarMensagem(this + " movido para (" + x + ", " + y + ").");
    }

    /**
     * Obtém a coordenada X do objeto.
     *
     * @return a coordenada X
     */
    @Override
    public int getPosX() {
        return posX; 
    }

    /**
     * Obtém a coordenada Y do objeto.
     *
     * @return a coordenada Y
     */
    @Override
    public int getPosY() {
        return posY; 
    }

    /**
     * Executa uma ação em um alvo.
     *
     * Este método é um espaço reservado para implementar ações que o 
     * objeto pode realizar em um alvo. Atualmente, não possui implementação.
     *
     * @param alvo o alvo sobre o qual a ação será realizada
     */
    @Override
    public void acao(IAtacavel alvo) {
        // Implementação da ação a ser realizada no alvo
    }

    /**
     * Obtém o custo de manutenção do objeto.
     *
     * Este método retorna o custo de manutenção associado ao objeto.
     *
     * @return o custo de manutenção
     */
    @Override
    public int getCustoManutencao() {
        return custoManutencao; 
    }
    /**
    * Obtém a descrição do objeto.
    *
    * Este método retorna uma string que contém a descrição do objeto, 
    * incluindo seu tipo, vida e força de combate.
    *
    * @return uma string representando a descrição do objeto
    */
   @Override
   public String getDescricao() {
       String texto = toString();
       texto += "\nVida: " + vida;
       texto += "\nForca de combate: " + forcaCombate;
       return texto;
   }

   /**
    * Obtém o símbolo representativo do objeto.
    *
    * Este método retorna o símbolo que representa o objeto no mapa.
    *
    * @return o símbolo do objeto
    */
   @Override
   public String getSimbolo() {
       return "CL"; 
   }

   /**
    * Retorna uma representação em string do objeto.
    *
    * Este método retorna uma string que representa o tipo do objeto, 
    * neste caso, "COLONO".
    *
    * @return uma string representando o objeto
    */
   @Override
   public String toString() {
       return "COLONO";
   }
}
