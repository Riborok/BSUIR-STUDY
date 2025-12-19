library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity sync_reg_struct is
    generic (
        N : integer
    );
    port (
        CLK, EN: in std_logic;
        Din: in std_logic_vector(N-1 downto 0);
        Dout: out std_logic_vector(N-1 downto 0)
    );
end sync_reg_struct;

architecture Structural of sync_reg_struct is
    component d_trigger_en is
        port (
            CLK, EN, Din: in std_logic;
            Dout: out std_logic
        );
    end component;
begin
    gen_triggers: for i in 0 to N-1 generate
        trigger_instance: d_trigger_en
            port map (
                CLK => CLK,       
                EN => EN,     
                Din => Din(i),   
                Dout => Dout(i)  
            );
    end generate;
end Structural;
