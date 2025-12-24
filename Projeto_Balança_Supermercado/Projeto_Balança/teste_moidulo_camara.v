`timescale 1ns / 1ps

////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer:
//
// Create Date:   00:47:28 12/20/2023
// Design Name:   modulo_camara
// Module Name:   C:/Users/joaoc/Desktop/PROJETOS/Projeto_F1v2/teste_moidulo_camara.v
// Project Name:  Projeto_F1v2
// Target Device:  
// Tool versions:  
// Description: 
//
// Verilog Test Fixture created by ISE for module: modulo_camara
//
// Dependencies:
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
////////////////////////////////////////////////////////////////////////////////

module teste_modulo_camara;

  // Inputs
  reg camera_output;

  // Outputs
  wire [2:0] product_detected;

  // Instantiate the Unit Under Test (UUT)
  modulo_camara uut (
    .camera_output(camera_output),
    .product_detected(product_detected)
  );

  initial begin
    // Initialize Inputs
    camera_output = 0;
    #20
    camera_output = 1;
    #20
    camera_output = 2;
    #20
    camera_output = 3;
    #150 $finish;
  end

endmodule
