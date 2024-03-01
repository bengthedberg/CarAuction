import { create } from "zustand";

type State = {
  pageNumber: number;
  pageSize: number;
  pageCount: number;
  searchTerm: string;
};

type Actions = {
  setParams: (params: Partial<State>) => void;
  reset: () => void;
};

const initialState: State = {
  pageNumber: 1,
  pageSize: 12,
  pageCount: 1,
  searchTerm: ""
};

export const useParamsStore = create<State & Actions>()((set) => ({
  ...initialState,

  setParams: (newParams: Partial<State>) => {
    set((state) => {
      // if pagenumber is updated
      if (newParams.pageNumber) {
        return { ...state, pageNumber: newParams.pageNumber };
      } else {
        // reset the page number for all other changes
        return { ...state, ...newParams, pageNumber: 1 };
      }
    });
  },

  reset: () => set(initialState)
}));
