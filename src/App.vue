<template>
  <div>
    <h2>Gestion des salles</h2>
    <RoomList
      :rows="rows"
      @update-rows="updateRows"
      @edit-row="editRow"
    />
    <hr />
    <RoomForm
      :editRowData="editRowData"
      @save-row="saveRow"
      @reset-form="resetForm"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import RoomList from "./components/RoomList.vue";
import RoomForm from "./components/RoomForm.vue";

export default defineComponent({
  name: "App",
  components: { RoomList, RoomForm },
  setup() {
    const rows = ref<{ id: string; name: string }[]>([]);
    const editRowData = ref<{ id: string; name: string } | null>(null);

    const saveRow = (row: { id: string; name: string }) => {
      if (row.id) {
        const index = rows.value.findIndex((r) => r.id === row.id);
        if (index > -1) {
          rows.value[index].name = row.name;
        }
      } else {
        rows.value.push({ id: (rows.value.length + 1).toString(), name: row.name });
      }
      editRowData.value = null;
    };

    const updateRows = (updatedRows: { id: string; name: string }[]) => {
      rows.value = updatedRows;
    };

    const editRow = (row: { id: string; name: string }) => {
      editRowData.value = row;
    };

    const resetForm = () => {
      editRowData.value = null;
    };

    return { rows, editRowData, saveRow, updateRows, editRow, resetForm };
  },
});
</script>
