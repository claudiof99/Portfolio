/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package com.mycompany.poo.civilizations;

import java.util.HashMap;

/**
 *
 * @author Diogo
 */
public class GestorRecursos {
    private final int MAX_RESERVA = 100;
    private final int MIN_RESERVA = 0;
    //HashMap não aceita duplicados nem ordena
    private HashMap<String, Integer> recursos; // Armazena tipo e quantidade de recurso
    private int alimentoNecessario, reservaComida, ouroFinalTurno;
    
    public GestorRecursos() {
        recursos = new HashMap<>();
        this.alimentoNecessario = 20;
        this.reservaComida = 0;
    }   
    /**
    * Verifica a reserva de comida de uma cidade e ajusta a população conforme necessário.
    *
    * Este método verifica se a reserva de comida é negativa ou positiva. Se for negativa, 
    * a reserva é ajustada para zero e a população é reduzida. Se for positiva, a população 
    * é aumentada enquanto houver comida suficiente na reserva.
    *
    * @param cidade a cidade cuja reserva de comida está sendo verificada
    * @param populacao a população da cidade que será ajustada
    */
   public void verificaReserva(Cidade cidade, Populacao populacao) { 
       int cont = 0;
       if (reservaComida < 0) {
           // Se a reserva de comida estiver negativa, ajusta para zero e reduz a população
           resetReservaComida(populacao);
       } else if (reservaComida > 0) {
           // Aumentar população enquanto há comida suficiente na reserva
           while (reservaComida >= MAX_RESERVA) {
               reservaComida -= MAX_RESERVA;            
               populacao.addTotal(1); // Incrementa a população 
               cont++;
           }
           if (cont == 0) {
               mostraPopulacao(populacao);
           } else { 
               GestorMensagens.mostrarMensagem("Populacao aumentou +" + cont + ": " + 
                       populacao.getTotal() + " elementos.");
           }
       }
       GestorMensagens.mostrarMensagem("Reserva de comida: " + reservaComida);
   }

   /**
    * Calcula a produção de recursos de uma cidade com base nos terrenos trabalhados.
    *
    * Este método itera sobre os terrenos trabalhados pela cidade e calcula a produção 
    * de recursos com base no número de trabalhadores alocados em cada terreno.
    *
    * @param cidade a cidade cuja produção de recursos está sendo calculada
    */
   public void calcularProducao(Cidade cidade) {
       for (ITerreno terreno : cidade.getTerrenosTrabalhados()) {
           int numTrabalhadores = terreno.getCidadaosTrabalhando();
           for (IRecurso recurso : terreno.getRecursos()) {
               // Produção é proporcional ao número de cidadãos alocados
               int producao = recurso.getQuantidade() * numTrabalhadores;
               adicionarRecurso(recurso.getTipoRecurso(), producao);
           }
       }
   }

   /**
    * Reseta a produção de recursos de uma cidade.
    *
    * Este método limpa as variáveis que guardam os recursos gerados durante o turno 
    * atual, definindo a produção de cada recurso como zero.
    *
    * @param cidade a cidade cuja produção está sendo resetada
    */
   public void resetProducao(Cidade cidade) {    
       ouroFinalTurno = recursos.getOrDefault("Ouro", 0);

       for (ITerreno terreno : cidade.getTerrenosTrabalhados()) {
           for (IRecurso recurso : terreno.getRecursos()) {
               // Limpar as variáveis que guardam os recursos gerados
               recursos.put(recurso.getTipoRecurso(), 0);
           }
       }
   }

   /**
    * Obtém a quantidade de alimento necessária.
    *
    * @return a quantidade de alimento necessária
    */
   public int getAliNecessario() {
       return alimentoNecessario;
   }

   /**
    * Define a quantidade de alimento necessária.
    *
    * @param necessidade a nova quantidade de alimento necessária
    */
   public void setAliNecessario(int necessidade) {
       alimentoNecessario = necessidade;
   }

   /**
    * Obtém a reserva de comida atual.
    *
    * @return a quantidade atual de reserva de comida
    */
   public int getReservaComida() {
       return reservaComida;
   }

   /**
    * Define a reserva de comida, adicionando a quantidade especificada.
    *
    * @param reservaComida a quantidade de comida a ser adicionada à reserva
    */
   public void setReservaComida(int reservaComida) {
       this.reservaComida += reservaComida;
   }

   /**
    * Reseta a reserva de comida e reduz a população se a comida for insuficiente.
    *
    * Este método ajusta a reserva de comida para zero e remove um cidadão da 
    * população, exibindo uma mensagem informativa.
    *
    * @param populacao a população da cidade que será ajustada
    */
   public void resetReservaComida(Populacao populacao) {
       reservaComida = 0;
       populacao.removerCidadao();
       GestorMensagens.mostrarMensagem("Comida insuficiente para todos os cidadaos."
               + "\nPopulacao diminuiu -1: " + populacao.getTotal() + 
               " elementos."); 
   }

   /**
    * Exibe a população atual da cidade.
    *
    * Este método exibe uma mensagem com a quantidade total de cidadãos na cidade.
    *
    * @param populacao a população da cidade a ser exibida
    */
    public void mostraPopulacao(Populacao populacao) {
        GestorMensagens.mostrarMensagem("Populacao: " + populacao.getTotal() + 
                        " elementos.");
    }
    
    /**
    * Adiciona uma quantidade de recurso ao tipo especificado.
    *
    * Este método atualiza a quantidade de um recurso específico, adicionando a 
    * quantidade fornecida. Se o recurso não existir, ele é criado com a 
    * quantidade especificada.
    *
    * @param tipo o tipo do recurso a ser adicionado
    * @param quantidade a quantidade de recurso a ser adicionada
    */
   public void adicionarRecurso(String tipo, int quantidade) {
       recursos.put(tipo, recursos.getOrDefault(tipo, 0) + quantidade);
   }

   /**
    * Consome uma quantidade de recurso do tipo especificado.
    *
    * Este método verifica se há recursos suficientes do tipo especificado para 
    * a quantidade a ser consumida. Se houver, a quantidade é subtraída e o 
    * método retorna true. Caso contrário, retorna false.
    *
    * @param tipo o tipo do recurso a ser consumido
    * @param quantidade a quantidade de recurso a ser consumida
    * @return true se a quantidade foi consumida com sucesso, false caso contrário
    */
   public boolean consumirRecurso(String tipo, int quantidade) {
       if (recursos.getOrDefault(tipo, 0) >= quantidade) {
           recursos.put(tipo, recursos.get(tipo) - quantidade);
           return true;
       }
       return false; // Não havia recursos suficientes
   }

   /**
    * Obtém a quantidade de ouro sobrando no final do turno.
    *
    * @return a quantidade de ouro restante no final do turno
    */
   public int getSobraOuro() {
       return ouroFinalTurno;
   }

   /**
    * Consulta a quantidade de um recurso específico.
    *
    * Este método retorna a quantidade atual de um recurso do tipo especificado. 
    * Se o recurso não existir, retorna zero.
    *
    * @param tipo o tipo do recurso a ser consultado
    * @return a quantidade do recurso especificado
    */
   public int getQuantidade(String tipo) {
       return recursos.getOrDefault(tipo, 0);
   }

   /**
    * Obtém um mapa contendo todos os recursos e suas quantidades.
    *
    * Este método retorna uma cópia do mapa de recursos, onde as chaves são os 
    * tipos de recursos e os valores são as quantidades correspondentes.
    *
    * @return um HashMap contendo todos os recursos e suas quantidades
    */
   public HashMap<String, Integer> getRecursos() {
       return new HashMap<String, Integer>(recursos);
   }

   /**
    * Retorna um resumo de todos os recursos gerados.
    *
    * Este método cria uma representação em string de todos os recursos e suas 
    * quantidades, formatando-os de maneira legível.
    *
    * @return uma string representando todos os recursos gerados
    */
   @Override
   public String toString() {
       // Evita a criação contínua de Strings, torna o método mais eficiente
       StringBuilder sb = new StringBuilder();
       sb.append("--- Recursos gerados: ---\n");
       recursos.forEach((tipo, quantidade) -> {
           sb.append(tipo).append(": ").append(quantidade).append(" | ");
       });
       return sb.toString();
   }
}
