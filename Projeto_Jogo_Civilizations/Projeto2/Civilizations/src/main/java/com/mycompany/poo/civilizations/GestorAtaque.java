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
public class GestorAtaque implements IAcao{
    private IMapa mapa;
    private Scanner s;
    
    public GestorAtaque(IMapa mapa) {
        this.mapa = mapa;
        s = new Scanner(System.in); 
    }
    
    @Override
    /**
    * Executa a ação de uma unidade militar.
    *
    * Este método permite que uma unidade militar execute uma ação, como atacar 
    * um alvo. O jogador seleciona a unidade e, em seguida, escolhe um alvo 
    * próximo para atacar.
    *
    * @param tipoUnidade o tipo da unidade que está realizando a ação
    * @param listaUnidades a lista de unidades disponíveis para seleção
    */
    public void acaoUnidade(String tipoUnidade, List<IUnidade> listaUnidades) {
        IUnidade unidadeSelecionada = selecionarUnidade(listaUnidades);
        if (unidadeSelecionada == null) {
            return;
        }
        if (unidadeSelecionada.isOcupada()) {
            GestorMensagens.mostrarMensagem("A unidade escolhida esta ocupada.");
            return;
        }
        mapa.mostrarMapaZoom(unidadeSelecionada.getPosX(), unidadeSelecionada.getPosY(), 2);
        GestorMensagens.mostrarMensagem("Posicao atual: (" + unidadeSelecionada.getPosX() + 
            ", " + unidadeSelecionada.getPosY() + ")");
        List<IAtacavel> alvos = identificarAlvos(unidadeSelecionada);

        if (alvos.isEmpty()) {
            GestorMensagens.mostrarMensagem("Nao ha alvos proximos para atacar.");
            return;
        }
        IAtacavel alvoSelecionado = selecionarAlvo(alvos);
        if (alvoSelecionado == null) {
            return;
        }
        realizarAtaque(unidadeSelecionada, alvoSelecionado);
    }  
    /**
    * Seleciona uma unidade da lista de unidades disponíveis.
    *
    * Este método solicita ao jogador que escolha uma unidade militar a partir 
    * da lista fornecida. Se a escolha for inválida, uma mensagem de erro é 
    * exibida.
    *
    * @param listaUnidades a lista de unidades disponíveis para seleção
    * @return a unidade selecionada ou null se a escolha for inválida
    */
   private IUnidade selecionarUnidade(List<IUnidade> listaUnidades) {
       GestorMensagens.mostrarMensagem("Escolha a unidade militar para realizar o ataque:");
       int escolha = s.nextInt();
       s.nextLine(); // Limpa o buffer

       if (escolha < 1 || escolha > listaUnidades.size()) {
           GestorMensagens.mostrarMensagem("Escolha invalida!");
           return null;
       }
       return listaUnidades.get(escolha - 1);
   }

   /**
    * Identifica alvos que podem ser atacados pela unidade selecionada.
    *
    * Este método verifica os terrenos adjacentes à posição da unidade e 
    * retorna uma lista de alvos que podem ser atacados, excluindo aqueles 
    * que pertencem à mesma civilização da unidade selecionada.
    *
    * @param unidadeSelecionada a unidade que está realizando a identificação de alvos
    * @return uma lista de alvos que podem ser atacados
    */
   private List<IAtacavel> identificarAlvos(IUnidade unidadeSelecionada) {
       List<ITerreno> terrenosAdjacentes = mapa.getTerrenosProximos(unidadeSelecionada.getPosX(), 
               unidadeSelecionada.getPosY(), 1);

       List<IAtacavel> alvos = new ArrayList<>();
       for (ITerreno terreno : terrenosAdjacentes) {
           for (IAtacavel alvo : terreno.getAlvos()) {
               if (alvo instanceof IUnidade) {
                   if (!((IUnidade) alvo).getCidade().getCivilizacao().equals(unidadeSelecionada.getCidade().getCivilizacao())) {
                       alvos.add(alvo);
                   }
               } else if (alvo instanceof Cidade && 
                       !((Cidade) alvo).getCivilizacao().equals(unidadeSelecionada.getCidade().getCivilizacao())) {
                   alvos.add(alvo);
               }
           }
       }
       return alvos;
   }

   /**
    * Seleciona um alvo a partir da lista de alvos disponíveis.
    *
    * Este método exibe os alvos próximos e solicita ao jogador que escolha 
    * um deles. Se a escolha for inválida, uma mensagem de erro é exibida.
    *
    * @param alvos a lista de alvos disponíveis para seleção
    * @return o alvo selecionado ou null se a escolha for inválida
    */
   private IAtacavel selecionarAlvo(List<IAtacavel> alvos) {
       GestorMensagens.mostrarMensagem("Alvos proximos:");
       for (int i = 0; i < alvos.size(); i++) {
           IAtacavel alvo = alvos.get(i);
           GestorMensagens.mostrarMensagem((i + 1) + ". " + alvo.getDescricao());
       }

       GestorMensagens.mostrarMensagem("Escolha o numero do alvo:");
       int escolha = s.nextInt();
       s.nextLine(); // Limpa o buffer

       if (escolha < 1 || escolha > alvos.size()) {
           GestorMensagens.mostrarMensagem("Escolha invalida!");
           return null;
       }

       return alvos.get(escolha - 1);
   }

   /**
    * Identifica unidades que podem ser capturadas pela unidade selecionada.
    *
    * Este método verifica os alvos próximos e retorna uma lista de unidades 
    * que podem ser capturadas, considerando apenas unidades do tipo "COLONO" 
    * ou "CONSTRUTOR" que pertencem a uma civilização diferente.
    *
    * @param unidadeSelecionada a unidade que está realizando a identificação de unidades capturáveis
    * @return uma lista de unidades capturáveis
    */
   private List<IUnidade> identificarUnidadesCapturaveis(IUnidade unidadeSelecionada) {
       List<IAtacavel> alvos = identificarAlvos(unidadeSelecionada);

       List<IUnidade> unidadesCapturaveis = new ArrayList<>();
       for (IAtacavel alvo : alvos) {
           if (alvo instanceof IUnidade unidade && 
               (unidade.toString().equals("COLONO") || unidade.toString().equals("CONSTRUTOR")) &&
               !unidade.getCidade().getCivilizacao().equals(unidadeSelecionada.getCidade().getCivilizacao())) {
               unidadesCapturaveis.add(unidade);
           }
       }
       return unidadesCapturaveis;
   }
    
    /**
    * Realiza um ataque de uma unidade a um alvo.
    *
    * Este método executa a ação de ataque da unidade selecionada contra o 
    * alvo especificado. Após a execução do ataque, uma mensagem de sucesso 
    * é exibida.
    *
    * @param unidade a unidade que está realizando o ataque
    * @param alvo o alvo que está sendo atacado
    */
   private void realizarAtaque(IUnidade unidade, IAtacavel alvo) {
       unidade.acao(alvo);
       GestorMensagens.mostrarMensagem("Ataque realizado com sucesso!");
   }

   /**
    * Realiza a captura de uma unidade inimiga.
    *
    * Este método permite que uma unidade selecionada tente capturar uma unidade 
    * inimiga próxima. O jogador escolhe a unidade a ser utilizada para a captura, 
    * e se a unidade estiver ocupada ou não houver unidades capturáveis, 
    * mensagens apropriadas são exibidas.
    *
    * @param listaUnidades a lista de unidades disponíveis para seleção
    */
   public void realizarCaptura(List<IUnidade> listaUnidades) {
       IUnidade unidadeSelecionada = selecionarUnidade(listaUnidades);
       if (unidadeSelecionada == null) {
           return;
       }
       if (unidadeSelecionada.isOcupada()) {
           GestorMensagens.mostrarMensagem("A unidade escolhida esta ocupada.");
           return;
       }

       mapa.mostrarMapaZoom(unidadeSelecionada.getPosX(), unidadeSelecionada.getPosY(), 2);
       GestorMensagens.mostrarMensagem("Posicao atual: (" + unidadeSelecionada.getPosX() + 
           ", " + unidadeSelecionada.getPosY() + ")");
       List<IUnidade> alvosCapturaveis = identificarUnidadesCapturaveis(unidadeSelecionada);

       if (alvosCapturaveis.isEmpty()) {
           GestorMensagens.mostrarMensagem("Nao ha unidades capturaveis proximas.");
           return;
       }
       // Converte a lista de IUnidade para List<IAtacavel>
       List<IAtacavel> alvosComoAtacaveis = new ArrayList<>(alvosCapturaveis);
       // Passa a lista convertida para selecionarAlvo
       IAtacavel alvoSelecionado = selecionarAlvo(alvosComoAtacaveis);
       if (alvoSelecionado == null) {
           return;
       }
       // Verifica se o alvo selecionado é uma unidade capturável
       if (alvoSelecionado instanceof IUnidade unidadeCapturada) {
           capturarUnidade(unidadeSelecionada, unidadeCapturada);
       } else {
           GestorMensagens.mostrarMensagem("O alvo selecionado nao e valido para captura.");
       }
   }

   /**
    * Captura uma unidade inimiga.
    *
    * Este método remove a unidade capturada da civilização original e a 
    * adiciona à civilização da unidade militar que está realizando a captura. 
    * Uma mensagem de sucesso é exibida após a captura.
    *
    * @param unidadeMilitar a unidade que está realizando a captura
    * @param unidadeCapturada a unidade que está sendo capturada
    */
   private void capturarUnidade(IUnidade unidadeMilitar, IUnidade unidadeCapturada) {
       unidadeCapturada.getCidade().getGestorUnidades().getUnidades().remove(unidadeCapturada.toString(), unidadeCapturada);
       unidadeMilitar.getCidade().getGestorUnidades().getUnidades().get(unidadeCapturada.toString()).add(unidadeCapturada);
       unidadeCapturada.setCidade(unidadeMilitar.getCidade());
       GestorMensagens.mostrarMensagem("Unidade " + unidadeCapturada.toString() + 
               " capturada com sucesso!");
   }
}
