using System;
using Newtonsoft.Json;
using GameUnitSpace;

namespace AttackSpace {
    class AttackPosition {
        [JsonProperty]
        private int position;
        [JsonProperty]
        private Boolean onHorse;
        [JsonProperty]
        private readonly Character character; 

        public AttackPosition(Character c) { 
            this.character = c;
            this.onHorse = true;
            this.position = 0;
        }

        public void incrementPosition() {
            this.position += 1;
        }

        public void getOffHorse() {
            this.onHorse = false;
        }
    }
}