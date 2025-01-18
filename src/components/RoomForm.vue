<template>
  <form>
    <div>
      <input type="hidden" v-model="rowId" />
    </div>
    <div>
      <label for="nom">Nom</label>
      <input
        type="text"
        id="nom"
        placeholder="Tapez votre nom..."
        v-model="name"
        required
      />
    </div>
    <div>
      <label for="file-input">Photo</label>
      <input
        type="file"
        id="file-input"
        accept="image/*"
        @change="handleFileChange"
      />
      <div>
        <div class="drop-preview">
          <div
            id="drop-zone"
            @dragenter.prevent="hover = true"
            @dragleave.prevent="hover = false"
            @dragover.prevent
            @drop.prevent="handleDrop"
            :class="{ hover }"
          >
            Déposez votre image ici
          </div>
          <img v-if="imagePreview" :src="imagePreview" alt="Aperçu" />
        </div>
        <button type="button" @click="removePicture">Supprimer l'image</button>
      </div>
    </div>
    <div class="buttons-menu">
      <button type="button" @click="saveRow">Ajouter</button>
      <button type="button" @click="resetForm">Annuler</button>
    </div>
  </form>
</template>

<script lang="ts">
import { defineComponent, ref, watch } from "vue";

export default defineComponent({
  name: "RoomForm",
  props: {
    editRowData: {
      type: Object as () => { id: string; name: string } | null,
      default: null,
    },
  },
  emits: ["save-row", "reset-form"],
  setup(props, { emit }) {
    const rowId = ref("");
    const name = ref("");
    const imagePreview = ref<string | null>(null);
    const hover = ref(false);

    watch(
      () => props.editRowData,
      (newVal) => {
        if (newVal) {
          rowId.value = newVal.id;
          name.value = newVal.name;
        } else {
          resetForm();
        }
      }
    );

    const saveRow = () => {
      if (name.value.trim() === "") {
        alert("Veuillez entrer un nom.");
        return;
      }
      emit("save-row", { id: rowId.value, name: name.value });
      resetForm();
    };

    const resetForm = () => {
      rowId.value = "";
      name.value = "";
      imagePreview.value = null;
      emit("reset-form");
    };

    const handleDrop = (e: DragEvent) => {
      const file = e.dataTransfer?.files[0];
      if (file?.type.startsWith("image/")) {
        readFile(file);
      } else {
        alert("Veuillez déposer un fichier image.");
      }
    };

    const handleFileChange = (e: Event) => {
      const target = e.target as HTMLInputElement;
      const file = target.files?.[0];
      if (file?.type.startsWith("image/")) {
        readFile(file);
      } else {
        alert("Veuillez sélectionner un fichier image.");
      }
    };

    const readFile = (file: File) => {
      const reader = new FileReader();
      reader.onload = () => {
        imagePreview.value = reader.result as string;
      };
      reader.readAsDataURL(file);
    };

    const removePicture = () => {
      imagePreview.value = null;
    };

    return {
      rowId,
      name,
      imagePreview,
      hover,
      saveRow,
      resetForm,
      handleDrop,
      handleFileChange,
      removePicture,
    };
  },
});
</script>

<style scoped>
table {
  width: 60%;
  margin: 20px auto;
  border-collapse: collapse;
}

th,
td {
  border: 1px solid #ccc;
  padding: 10px;
  text-align: center;
}

th {
  background-color: #f2f2f2;
}

tbody tr:hover {
  background-color: #f0f0f0;
}

tbody tr.selected {
  background-color: #d0e7ff;
}

button {
  margin: 5px;
}

form {
  width: 60%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 10px;
  margin: 20px auto;
}

h2 {
  text-align: center;
}

hr {
  width: 60%;
}

.buttons-menu {
  text-align: center;
}

#delete-all {
  display: block;
  margin: 20px auto;
}

/* Drag & Drop styles */
.drop-preview {
  display: flex;
  flex-direction: row;
  align-items: center;
}

#file-input {
  margin: 20px auto;
  font-size: 16px;
}

#image-preview {
  margin: 20px auto;
  max-width: 100%;
  height: auto;
}

#drop-zone {
  width: 300px;
  height: 200px;
  border: 2px dashed #ccc;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  color: #aaa;
  text-align: center;
}

#drop-zone.hover {
  border-color: #333;
  color: #333;
}

/*# sourceMappingURL=index.css.map */
</style>
