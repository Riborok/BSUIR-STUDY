library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity async_reg_struct is
    generic (
        N: integer
    );
    port (
        EN: in std_logic;
        Din: in std_logic_vector(N-1 downto 0);
        Dout: out std_logic_vector(N-1 downto 0)
    );
end async_reg_struct;

architecture Structural of async_reg_struct is
    component d_latch_en is
        port (
            EN, Din: in std_logic;
            Dout: out std_logic
        );
    end component; 
begin
    gen_latches: for i in 0 to N-1 generate
        latch_instance: d_latch_en
            port map (
                EN => EN,     
                Din => Din(i),      
                Dout => Dout(i)     
            );
    end generate;

end Structural;
