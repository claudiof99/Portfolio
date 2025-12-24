`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   01:32:00 12/22/2023
// Design Name:   bin2bcd_11_16
// Module Name:   C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/teste.v
// Project Name:  Projeto_F1v2
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: bin2bcd_11_16
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module teste;

	// Inputs
	reg [10:0] bin;

	// Outputs
	wire [15:0] bcd;

	// Instantiate the Unit Under Test (UUT)
	bin2bcd_11_16 uut (
		.bin(bin), 
		.bcd(bcd)
	);

	initial begin
		// Initialize Inputs
		bin = 2000;

		// Wait 100 ns for global reset to finish
		#100;
        
		// Add stimulus here
	  		  			  
	 #10 $finish;
end
endmodule
