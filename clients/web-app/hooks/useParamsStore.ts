import { shallow } from "zustand/shallow";
import { createWithEqualityFn } from "zustand/traditional";

type State = {
  pageNumber: number;
  pageSize: number;
  pageCount: number;
  searchTerm: string;
  searchValue: string;
};

type Actions = {
  setParams: (params: Partial<State>) => void;
  reset: () => void;
  setSearchValue: (value: string) => void;
};

const initialState: State = {
  pageNumber: 1,
  pageSize: 12,
  pageCount: 1,
  searchTerm: "",
  searchValue: "",
};

export const useParamsStore = createWithEqualityFn<State & Actions>()((set) => ({
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

  reset: () => set(initialState),

  setSearchValue: (value: string) => {
    set({ searchValue: value });
  },
}), shallow);
