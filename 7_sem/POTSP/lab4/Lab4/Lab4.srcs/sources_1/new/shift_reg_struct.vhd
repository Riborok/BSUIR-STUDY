library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity shift_reg_struct is
    generic (
        N: integer
    );
    port (
        CLK, RST, SE, Sin: in std_logic;                          
        Pout: out std_logic_vector(N-1 downto 0)
    );
end shift_reg_struct;

architecture Behavioral of shift_reg_struct is
    component d_trigger_ce is
        port (
            CLK, CLR, CE, D: in std_logic;
            Dout: out std_logic
        );
    end component;
    
    signal internal_reg: std_logic_vector(N downto 0);
begin
    internal_reg(N) <= Sin;

    gen_shift_reg: for i in 0 to N-1 generate
    begin
        trigger_instance: d_trigger_ce
            port map (
                CLK => CLK,
                CLR => RST,
                CE => SE,
                
                D => internal_reg(N-i),
                Dout => internal_reg(N-1-i)
            );
    end generate;
    
    Pout <= internal_reg(N-1 downto 0);
    
end Behavioral;
