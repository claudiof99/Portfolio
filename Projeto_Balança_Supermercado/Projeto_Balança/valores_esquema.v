// Verilog test fixture created from schematic C:\Users\joaoc\Desktop\PROJETOS\Projeto_F1v2\esquema.sch - Fri Nov 10 00:05:56 2023

`timescale 1ns / 1ps

module valores_esquema();

// Inputs
   reg funcao_tara;
	reg [1:0] produto;
   reg [10:0] peso_banana;
	reg [10:0] peso_maracuja;
	reg [10:0] peso_tangerina;
	reg [8:0] preco_banana;
	reg [8:0] preco_maracuja;
	reg [8:0] preco_tangerina;
	reg clk;
	reg rst;
	reg fim_compra;
	reg taxa;
	
// Output
   wire [3:0] BCDkg_decimal;
	wire [11:0] BCDkg_fracionario;
	wire [7:0] BCDeuros_decimal;
	wire [7:0] BCDeuros_fracionario;
	wire emissao_talao;
	wire [4:0] valor_taxa;

// Bidirs

// Instantiate the UUT
   esquema UUT (
	   .funcao_tara(funcao_tara),
      .produto(produto),		
		.peso_banana(peso_banana),
		.peso_maracuja(peso_maracuja),
		.peso_tangerina(peso_tangerina),
		.preco_banana(preco_banana),
		.preco_maracuja(preco_maracuja),
		.preco_tangerina(preco_tangerina),
		.BCDkg_decimal(BCDkg_decimal),
		.BCDkg_fracionario(BCDkg_fracionario),
		.BCDeuros_decimal(BCDeuros_decimal),
		.BCDeuros_fracionario(BCDeuros_fracionario),
		.clk(clk), 
		.rst(rst),
		.fim_compra(fim_compra), 
      .taxa(taxa),		
		.emissao_talao(emissao_talao),
		.valor_taxa(valor_taxa)
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
	 produto = 0;
	 peso_banana = 11'b0;
	 peso_maracuja = 11'b0;
	 peso_tangerina = 11'b0;
	 preco_banana = 9'b0;
	 preco_maracuja = 9'b0;
	 preco_tangerina = 9'b0; 
	 funcao_tara = 0;
	 taxa = 0;

    // Aguarda alguns ciclos
    #10 rst = 0;

// banana - 1
// maracuja - 2
// tangerina - 3

// Initialize Inputs

//  PRIMEIRO PRODUTO DO CABAZ
       peso_banana = 500;
		 preco_banana = 500;
		 
	    peso_maracuja = 500;
		 preco_maracuja = 300;
		 
	    peso_tangerina = 500;
	    preco_tangerina = 100;

           funcao_tara = 0;
			  produto = 1;

			  #10

//  SEGUNDO PRODUTO DO CABAZ			  
       peso_banana = 500;
		 preco_banana = 500;
		 
	    peso_maracuja = 500;
		 preco_maracuja = 300;
		 
	    peso_tangerina = 500;
	    preco_tangerina = 100;
			  
			  funcao_tara = 0;
			  produto = 2;

			  #10
			  		  
//  TERCEIRO PRODUTO DO CABAZ			  
       peso_banana = 500;
		 preco_banana = 500;
		 
	    peso_maracuja = 500;
		 preco_maracuja = 300;
		 
	    peso_tangerina = 500;
	    preco_tangerina = 100;
			  
			  funcao_tara = 0;
			  produto = 3;

			  #10			  
			  
//  QUARTO PRODUTO DO CABAZ			  
       peso_banana = 500;
		 preco_banana = 500;
		 
	    peso_maracuja = 500;
		 preco_maracuja = 300;
		 
	    peso_tangerina = 500;
	    preco_tangerina = 100;
			  
			  funcao_tara = 0;
			  produto = 3;

			  #10

	 // Ausencia de produtos			  
		#4  funcao_tara = 0;
			 produto=0;
			  
	 // Ativa a taxa
    #10 taxa = 1;
	 
    // Finaliza a compra
    #10 fim_compra = 1;		  
			  		  			  
	 #10 $finish;
end
endmodule
