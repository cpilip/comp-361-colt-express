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
        private readonly int maxPosition;

        public AttackPosition(Character c, int maxPos) { 
            this.character = c;
            this.onHorse = true;
            this.position = 0;
            this.maxPosition = maxPos;
        }

        public Boolean incrementPosition() {
            if (this.position == maxPosition - 1) {
                this.onHorse = false;
                return false;
            }
            this.position += 1;
            return true;

        }

        public void getOffHorse() {
            this.onHorse = false;
        }

        public Boolean hasStopped() {
            return this.onHorse;
        }

        public Character GetCharacter() {
            return this.character;
        }

        public int getPosition() {
            return this.position;
        }
    }
}