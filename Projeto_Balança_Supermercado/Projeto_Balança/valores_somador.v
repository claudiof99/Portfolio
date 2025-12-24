`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   17:15:24 12/21/2023
// Design Name:   somador_preco
// Module Name:   C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/valores_somador.v
// Project Name:  Projeto_F1v2
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: somador_preco
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module valores_somador;

	// Inputs
	reg clk;
	reg rst;
	reg fim_compra;
	reg [10:0] preco_produto;
	reg [10:0] peso_produto;
	reg taxa;

	// Outputs
	wire [11:0] soma_final;
	wire [11:0] soma_peso;
	wire emissao_talao;
	wire [4:0] valor_taxa;

	// Instantiate the Unit Under Test (UUT)
	somador_preco_peso uut (
		.clk(clk), 
		.rst(rst),
		.fim_compra(fim_compra), 
		.preco_produto(preco_produto),
		.peso_produto(peso_produto),
      .taxa(taxa),		
		.soma_final(soma_final),
		.emissao_talao(emissao_talao),
		.valor_taxa(valor_taxa),
		.soma_peso(soma_peso)
	);
	
// Geração de clock
initial begin
    clk = 0;
    forever #5 clk = ~clk;
end

// Teste de simulação
initial begin
    // Inicializações
    rst = 1;
    fim_compra = 0;
    preco_produto = 11'b0;
	 peso_produto = 11'b0;
	 taxa = 0;

    // Aguarda alguns ciclos
    #10 rst = 0;



    // Envia alguns valores de preço do produto
    preco_produto = 250; 
    peso_produto = 500;
	 
	 #10 preco_produto = 250;
	     peso_produto = 500;
    
	 #10 preco_produto = 250;
        peso_produto = 500;	 
	 
	 #10 preco_produto = 250;
        peso_produto = 500;	 
	 
	 
	 // Ausencia de produtos
    #10 preco_produto = 0;
	     peso_produto = 0;
	 
    // Ativa a taxa
    #10 taxa = 1;
	 
    // Finaliza a compra
    #10 fim_compra = 1;

    #10;

    $finish;
end

endmodule
