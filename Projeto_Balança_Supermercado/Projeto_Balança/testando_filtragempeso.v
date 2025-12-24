`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   12:58:25 12/23/2023
// Design Name:   filtragem_peso
// Module Name:   C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/testando_filtragempeso.v
// Project Name:  Projeto_F1v2
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: filtragem_peso
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module testando_filtragempeso;

	// Inputs
	reg [1:0] produto;
	reg [10:0] peso_banana;
	reg [10:0] peso_maracuja;
	reg [10:0] peso_tangerina;
	reg tara;

	// Outputs
	wire [10:0] peso_liq;

	// Instantiate the Unit Under Test (UUT)
	filtragem_peso uut (
		.produto(produto), 
		.peso_banana(peso_banana), 
		.peso_maracuja(peso_maracuja), 
		.peso_tangerina(peso_tangerina), 
		.tara(tara), 
		.peso_liq(peso_liq)
	);

	initial begin
		// Initialize Inputs
		produto = 2;
		peso_banana = 100;
		peso_maracuja = 200;
		peso_tangerina = 300;
		tara = 1;

		// Wait 100 ns for global reset to finish
		#100;
        
		// Add stimulus here
     $finish;
	end
 
endmodule
