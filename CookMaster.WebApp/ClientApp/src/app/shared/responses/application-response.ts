
export type ApplicationSuccessResponse<T> = T & {
  Success: true;
};

export type ApplicationErrorResponse = {
  Success: false;
  Message: string;
}

export type ApplicationResponse<T> = ApplicationSuccessResponse<T> | ApplicationErrorResponse;
