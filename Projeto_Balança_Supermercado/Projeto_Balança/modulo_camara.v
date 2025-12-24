`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    00:45:09 12/20/2023 
// Design Name: 
// Module Name:    modulo_camara 
// Project Name: 
// Target Devices: 
// Tool versions: 
// Description: 
//
// Dependencies: 
//
// Revision: 
// Revision 0.01 - File Created
// Additional Comments: 
//
//////////////////////////////////////////////////////////////////////////////////
module CameraModule(
    input wire [2:0] camera_output,
    output reg [2:0] product_detected
);

// Parâmetros de identificação dos produtos
parameter BANANA_ID = 3'b001;
parameter MARACUJA_ID = 3'b010;
parameter TANGERINA_ID = 3'b100;

// Lógica de detecção de produtos (exemplo simplificado)
always @(posedge camera_output) begin
    case (camera_output)
        BANANA_ID: product_detected <= BANANA_ID;
        MARACUJA_ID: product_detected <= MARACUJA_ID;
        TANGERINA_ID: product_detected <= TANGERINA_ID;
        default: product_detected <= 3'b000; // Produto não reconhecido
    endcase
end

endmodule

