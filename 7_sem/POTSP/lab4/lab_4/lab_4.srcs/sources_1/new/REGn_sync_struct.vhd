----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 16:56:13
-- Design Name: 
-- Module Name: REGn_sync_struct - Structural
-- Project Name: 
-- Target Devices: 
-- Tool Versions: 
-- Description: 
-- 
-- Dependencies: 
-- 
-- Revision:
-- Revision 0.01 - File Created
-- Additional Comments:
-- 
----------------------------------------------------------------------------------


library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity REGn_sync_struct is
    generic (N : integer := 8);
    port (
        Din  : in  std_logic_vector(N-1 downto 0);
        CLK  : in  std_logic;
        EN   : in  std_logic;
        Dout : out std_logic_vector(N-1 downto 0)
    );
end REGn_sync_struct;

architecture Structural of REGn_sync_struct is
    component DE_trigger
        port (
            Din, CLK, EN: in std_logic;
            Dout: out std_logic
        );
    end component;
begin
    gen_de_trig : for i in 0 to N-1 generate
        de_trig : DE_trigger port map(
            Din => Din(i),
            CLK => CLK,
            EN => EN,
            Dout => Dout(i)
        );
    end generate;
end Structural;
