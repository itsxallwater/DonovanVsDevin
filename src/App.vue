<template>
  <div>
    <div class="flex items-center flex-wrap mx-auto w-full sm:w-3/5">
      <PlayerCard
        class="w-full sm:w-2/5 px-4"
        :player="players[0]"
        @per-received="perReceived"
      />
      <div class="w-full sm:w-1/5 px-4" id="ball">VS</div>
      <PlayerCard
        class="w-full sm:w-2/5 px-4"
        :player="players[1]"
        @per-received="perReceived"
      />
    </div>
    <div class="flex items-center mx-auto w-full sm:w-2/4">
      <Thanks class="w-full" />
    </div>
    <Footer class="p-8" />
  </div>
</template>

<script type="ts">
import { defineComponent } from "vue";

import PlayerCard from "./components/PlayerCard.vue";
import Thanks from "./components/Thanks.vue";
import Footer from "./components/Footer.vue";

export default defineComponent({
  name: "App",
  components: {
    PlayerCard,
    Thanks,
    Footer,
  },
  data() {
    return {
      players: [
        {
          id: 3908809,
          name: "Donovan Mitchell",
          per: undefined,
          winner: false,
          team: "utah",
          image: "./donovan.png",
        },
        {
          id: 3136193,
          name: "Devin Booker",
          per: undefined,
          winner: false,
          team: "phoenix",
          image: "./devin.png",
        },
      ],
    };
  },
  methods: {
    perReceived(value) {
      this.players.find((player) => player.id === value.id).per = value.per;
      this.players.map((player) => (player.winner = false));
      const winner = (this.players.reduce(
        (king, player) => (!king || king.per < player.per ? player : king),
        null
      ).winner = true);
    },
  },
});
</script>
