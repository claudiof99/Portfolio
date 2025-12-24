`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   00:58:48 12/20/2023
// Design Name:   CameraModule
// Module Name:   C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/testa_modulo_camara.v
// Project Name:  Projeto_F1v2
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: CameraModule
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module testa_modulo_camara;

	// Inputs
	reg [2:0] camera_output;

	// Outputs
	wire [2:0] product_detected;

	// Instantiate the Unit Under Test (UUT)
	CameraModule uut (
		.camera_output(camera_output), 
		.product_detected(product_detected)
	);

	initial begin
		// Initialize Inputs
		camera_output = 0;
		#50
		camera_output = 1;
		#50
		camera_output = 2;
		#50
		camera_output = 3;
		#100;
       
	end   
endmodule

