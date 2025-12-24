`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date:    17:56:15 11/09/2023 
// Design Name: 
// Module Name:    ent_peso 
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
module filtragem_peso(
  input wire [1:0] produto,
  input wire [10:0] peso_banana,
  input wire [10:0] peso_maracuja,
  input wire [10:0] peso_tangerina,
  input wire tara,
  
  output reg [10:0] peso_liq 
);

  reg [10:0] peso_bruto;

  always @* begin
    case (produto)
      2'b01: peso_bruto = peso_banana;
      2'b10: peso_bruto = peso_maracuja;
      2'b11: peso_bruto = peso_tangerina;
      default: peso_bruto = 0; // Valor padrão caso o produto não seja 1, 2 ou 3
    endcase
  end

  always @* begin 
    if (tara && (peso_bruto >= 11'b111100 && peso_bruto <= 11'b11111010000)) begin // Se a tara for verdadeira e o peso bruto for superior ou igual a 60g e inferior ou igual a 2000g
      peso_liq = peso_bruto - 11'b111100; // Então o peso líquido é igual ao peso bruto menos 60g
		
    end else if (tara && (peso_bruto < 11'b111100)) begin // Se a tara for verdadeira e o peso bruto for inferior a 60g
      peso_liq = 11'b0; // Então o peso líquido é 0g
		
	 end else if (tara && (peso_bruto > 11'b11111010000)) begin // Se a tara for verdadeira e o peso bruto for superior a 2000g
	   peso_liq = (11'b11110010100 - 11'b111100) + (peso_bruto - 11'b11110010100); // Então o peso líquido é igual a (2000g menos 60g) mais (peso bruto menos 2000g)
		
    end else if (peso_bruto <= 11'b11111010000) begin // Se o peso bruto for inferior ou igual a 2000g
      peso_liq = peso_bruto; // Então é igual ao peso líquido
		
    end else if (peso_bruto > 11'b11111010000) begin // Se o peso bruto for maior que 2000g
      peso_liq = 11'b11111010000; // O peso líquido é igual a 2000g
    end
  end


endmodule
