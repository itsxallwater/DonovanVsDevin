<template>
  <div>
    <h2 class="font-bold text-3xl" :class="player.team" itemprop="givenName">
      {{ player.name }}
    </h2>
    <img class="mx-auto" :src="player.image" :alt="player.name" />
    <div :class="player.winner ? 'ribbon-winner' : 'ribbon-loser'">
      <Suspense>
        <template #default>
          <PER :id="player.id" @per-calculated="perReceived" />
        </template>
        <template #fallback> loading... </template>
      </Suspense>
    </div>
  </div>
</template>

<script type="ts">
import { defineComponent } from "vue";

import PER from "./PER.vue";

export default defineComponent({
  components: {
    PER,
  },
  props: {
    player: {
      id: Number,
      name: String,
      winner: Boolean,
    },
  },
  methods: {
    perReceived(value) {
      this.$emit("per-received", { id: this.player.id, per: value });
    },
  },
  setup(props) {
    return {};
  },
});
</script>
