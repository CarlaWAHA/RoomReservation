<template>
  <div>
    <table id="tableau">
      <thead>
        <tr>
          <th>
            <label for="select-all">Tout sélectionner</label>
            <input
              type="checkbox"
              id="select-all"
              v-model="selectAll"
              @change="toggleAll"
            />
          </th>
          <th>Id</th>
          <th>Nom</th>
          <th>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="row in rows"
          :key="row.id"
          :class="{ selected: row.id === selectedRow }"
          @click="selectRow(row)"
        >
          <td>
            <input
              type="checkbox"
              class="row-checkbox"
              v-model="selectedRows"
              :value="row.id"
            />
          </td>
          <td>{{ row.id }}</td>
          <td>{{ row.name }}</td>
          <td>
            <button type="button" @click.stop="removeRow(row.id)">Supprimer</button>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="buttons-menu">
      <button type="button" @click="removeSelectedRows">
        Supprimer les lignes sélectionnées
      </button>
      <button type="button" @click="saveTable">Sauvegarder le Tableau</button>
      <button type="button" @click="loadTable">Charger le Tableau</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, watch } from "vue";

export default defineComponent({
  name: "RoomList",
  props: {
    rows: {
      type: Array as () => { id: string; name: string }[],
      required: true,
    },
  },
  emits: ["update-rows", "edit-row"],
  setup(props, { emit }) {
    const selectedRows = ref<string[]>([]);
    const selectAll = ref(false);
    const selectedRow = ref<string | null>(null);

    const toggleAll = () => {
      selectedRows.value = selectAll.value
        ? props.rows.map((row) => row.id)
        : [];
    };

    const selectRow = (row: { id: string; name: string }) => {
      selectedRow.value = row.id;
      emit("edit-row", row);
    };

    const removeRow = (id: string) => {
      const updatedRows = props.rows.filter((row) => row.id !== id);
      emit("update-rows", updatedRows);
      selectedRows.value = [];
    };

    const removeSelectedRows = () => {
      const updatedRows = props.rows.filter(
        (row) => !selectedRows.value.includes(row.id)
      );
      emit("update-rows", updatedRows);
      selectedRows.value = [];
      selectAll.value = false;
    };

    const saveTable = () => {
      localStorage.setItem("tableauData", JSON.stringify(props.rows));
      alert("Tableau sauvegardé !");
    };

    const loadTable = () => {
      const data = localStorage.getItem("tableauData");
      if (data) {
        emit("update-rows", JSON.parse(data));
        alert("Tableau chargé !");
      } else {
        alert("Aucune donnée trouvée dans le Local Storage.");
      }
    };

    watch(() => props.rows, toggleAll, { immediate: true });

    return {
      selectedRows,
      selectAll,
      selectedRow,
      toggleAll,
      selectRow,
      removeRow,
      removeSelectedRows,
      saveTable,
      loadTable,
    };
  },
});
</script>

<style scoped>

</style>
