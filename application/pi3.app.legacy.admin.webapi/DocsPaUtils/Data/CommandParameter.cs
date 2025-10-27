// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace DocsPaUtils.Data
{

    public class CommandParameter
    {
        private string nome;
        private object valore;
        private int dimensione;
        private System.Data.DbType tipo;
        private DirectionParameter direzioneparametro;

        public CommandParameter(string nome, object valore)
		{
            this.nome = nome;
            this.valore = valore;
            this.dimensione = 0;
            this.direzioneparametro = DirectionParameter.ParamInput;
        }
        public CommandParameter(string nome, object valore, System.Data.DbType tipo)
        {
            this.nome = nome;
            this.valore = valore;
            this.dimensione = 0;
            this.direzioneparametro = DirectionParameter.ParamInput;
            this.tipo = tipo;
        }
        public string Nome
        {
            get { return nome; }
        }

        public object Valore
        {
            get { return valore; }
        }

        public int Dimensione
        {
            get { return dimensione; }
        }

        public System.Data.DbType Tipo
        {
            get { return tipo; }
        }

        public DirectionParameter DirezioneParametro
        {
            get { return direzioneparametro; }
        }
    }
}
