<template>
  <span class="font-bold ribbon-content">
    {{ per }}
  </span>
</template>

<script type="ts">
import { defineComponent } from "vue";

import StatsService from "../services/StatsService.js";

export default defineComponent({
  props: {
    id: Number,
  },
  async setup(props, { emit }) {
    const url = import.meta.env.VITE_STATS_API_URL;
    const api = new StatsService(url, import.meta.env.VITE_STATS_API_KEY);
    const per = await api
      .getPER(props.id)
      .then((response) => {
        emit("per-calculated", response);
        return response;
      })
      .catch((error) => {
        emit("per-calculated", 0);
        return "Not available";
      });
    return { per };
  },
});
</script>
