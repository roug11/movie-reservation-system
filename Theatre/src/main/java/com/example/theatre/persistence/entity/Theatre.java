package com.example.theatre.persistence.entity;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Data
@Table(name = "theater")
public class Theatre {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    private String name;
    private String address;
}